using System;
using System.Collections.Generic;
using System.Drawing;
using Liquid;

namespace LiquidSim
{
    /// <summary>
    /// Liquid simulator; based on code from http://grantkot.com/MPM/Liquid.html
    /// </summary>
    public class Liquid
    {
        private readonly List<Node> _active = new List<Node>();
        private readonly Node[,] _grid = new Node[GsizeX,GsizeY];
        private readonly List<Particle> _particles = new List<Particle>();
        private readonly Material _water = new Material(1.0F, 1.0F, 1.0F, 1.0F, 1.0F, 1.0F);

        private const int GsizeX = 240;
        private const int GsizeY = 150;

        private int _mx;
        private int _mxprev;
        private int _my;
        private int _myprev;
        private bool _pressed;
        private bool _pressedprev;
        private Pen _pen;
        private Random _rand = new Random();

        public void Init(int width, int height)
        {
            _pen = new Pen(Color.Blue);

            for (var i = 0; i < GsizeX; i++)
            {
                for (var j = 0; j < GsizeY; j++)
                {
                    _grid[i,j] = new Node();
                }
            }

            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    var p = new Particle(_water, i + 4, j + 4, 0.0F, 0.0F);
                    _particles.Add(p);
                }
            }
        }

        public void Paint(Graphics g)
        {
            g.Clear(Color.Wheat);

            foreach (var p in _particles)
            {
                g.DrawLine(_pen, (int) (4.0F*p.x), (int) (4.0F*p.y),
                                            (int) (4.0F*(p.x - p.u)), (int) (4.0F*(p.y - p.v)));
            }
        }

        public void Simulate()
        {
            var drag = false;
            var mdx = 0.0F;
            var mdy = 0.0F;

            if ((_pressed) && (_pressedprev))
            {
                drag = true;
                mdx = 0.25F*(_mx - _mxprev);
                mdy = 0.25F*(_my - _myprev);
            }

            _pressedprev = _pressed;
            _mxprev = _mx;
            _myprev = _my;

            foreach (var n in _active)
            {
                n.m = (n.d = n.gx = n.gy = n.u = n.v = n.ax = n.ay = 0.0F);
                n.active = false;
            }
            _active.Clear();

            foreach (var p in _particles)
            {
                p.cx = (int) (p.x - 0.5F);
                p.cy = (int) (p.y - 0.5F);

                float x = p.cx - p.x;
                p.px[0] = (0.5F*x*x + 1.5F*x + 1.125F);
                p.gx[0] = (x + 1.5F);
                x += 1.0F;
                p.px[1] = (-x*x + 0.75F);
                p.gx[1] = (-2.0F*x);
                x += 1.0F;
                p.px[2] = (0.5F*x*x - 1.5F*x + 1.125F);
                p.gx[2] = (x - 1.5F);

                float y = p.cy - p.y;
                p.py[0] = (0.5F*y*y + 1.5F*y + 1.125F);
                p.gy[0] = (y + 1.5F);
                y += 1.0F;
                p.py[1] = (-y*y + 0.75F);
                p.gy[1] = (-2.0F*y);
                y += 1.0F;
                p.py[2] = (0.5F*y*y - 1.5F*y + 1.125F);
                p.gy[2] = (y - 1.5F);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int cxi = p.cx + i;
                        int cyj = p.cy + j;
                        Node n = _grid[cxi,cyj];
                        if (!n.active)
                        {
                            _active.Add(n);
                            n.active = true;
                        }
                        float phi = p.px[i]*p.py[j];
                        n.m += phi*p.mat.m;
                        n.d += phi;
                        float dx = p.gx[i]*p.py[j];
                        float dy = p.px[i]*p.gy[j];
                        n.gx += dx;
                        n.gy += dy;
                    }
                }
            }

            foreach (var p in _particles)
            {
                var cx = (int) p.x;
                var cy = (int) p.y;
                int cxi = cx + 1;
                int cyi = cy + 1;

                float p00 = _grid[cx,cy].d;
                float x00 = _grid[cx, cy].gx;
                float y00 = _grid[cx, cy].gy;
                float p01 = _grid[cx, cyi].d;
                float x01 = _grid[cx, cyi].gx;
                float y01 = _grid[cx, cyi].gy;
                float p10 = _grid[cxi, cy].d;
                float x10 = _grid[cxi, cy].gx;
                float y10 = _grid[cxi, cy].gy;
                float p11 = _grid[cxi, cyi].d;
                float x11 = _grid[cxi, cyi].gx;
                float y11 = _grid[cxi, cyi].gy;

                float pdx = p10 - p00;
                float pdy = p01 - p00;
                float C20 = 3.0F*pdx - x10 - 2.0F*x00;
                float C02 = 3.0F*pdy - y01 - 2.0F*y00;
                float C30 = -2.0F*pdx + x10 + x00;
                float C03 = -2.0F*pdy + y01 + y00;
                float csum1 = p00 + y00 + C02 + C03;
                float csum2 = p00 + x00 + C20 + C30;
                float C21 = 3.0F*p11 - 2.0F*x01 - x11 - 3.0F*csum1 - C20;
                float C31 = -2.0F*p11 + x01 + x11 + 2.0F*csum1 - C30;
                float C12 = 3.0F*p11 - 2.0F*y10 - y11 - 3.0F*csum2 - C02;
                float C13 = -2.0F*p11 + y10 + y11 + 2.0F*csum2 - C03;
                float C11 = x01 - C13 - C12 - x00;

                float u = p.x - cx;
                float u2 = u*u;
                float u3 = u*u2;
                float v = p.y - cy;
                float v2 = v*v;
                float v3 = v*v2;
                float density = p00 + x00*u + y00*v + C20*u2 + C02*v2 +
                                C30*u3 + C03*v3 + C21*u2*v + C31*u3*v + C12*
                                                                        u*v2 + C13*u*v3 + C11*u*v;

                float pressure = density - 1.0F;
                if (pressure > 2.0F)
                {
                    pressure = 2.0F;
                }

                float fx = 0.0F;
                float fy = 0.0F;

                if (p.x < 4.0F)
                    fx += p.mat.m*(4.0F - p.x);
                else if (p.x > GsizeX - 5)
                {
                    fx += p.mat.m*(GsizeX - 5 - p.x);
                }
                if (p.y < 4.0F)
                    fy += p.mat.m*(4.0F - p.y);
                else if (p.y > GsizeY - 5)
                {
                    fy += p.mat.m*(GsizeY - 5 - p.y);
                }

                if (drag)
                {
                    float vx = Math.Abs(p.x - 0.25F*_mx);
                    float vy = Math.Abs(p.y - 0.25F*_my);
                    if ((vx < 10.0F) && (vy < 10.0F))
                    {
                        float weight = p.mat.m*(1.0F - vx/10.0F)*(1.0F - vy/10.0F);
                        fx += weight*(mdx - p.u);
                        fy += weight*(mdy - p.v);
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Node n = _grid[(p.cx + i), (p.cy + j)];
                        float phi = p.px[i]*p.py[j];
                        float gx = p.gx[i]*p.py[j];
                        float gy = p.px[i]*p.gy[j];

                        n.ax += -(gx*pressure) + fx*phi;
                        n.ay += -(gy*pressure) + fy*phi;
                    }
                }
            }

            foreach (var n in _active)
            {
                if (n.m > 0.0F)
                {
                    n.ax /= n.m;
                    n.ay /= n.m;
                    n.ay += 0.03F;
                }
            }
            foreach (var p in _particles)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Node n = _grid[(p.cx + i), (p.cy + j)];
                        float phi = p.px[i]*p.py[j];
                        p.u += phi*n.ax;
                        p.v += phi*n.ay;
                    }
                }
                float mu = p.mat.m*p.u;
                float mv = p.mat.m*p.v;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Node n = _grid[(p.cx + i), (p.cy + j)];
                        float phi = p.px[i]*p.py[j];
                        n.u += phi*mu;
                        n.v += phi*mv;
                    }
                }
            }
            foreach (var n in _active)
            {
                if (n.m > 0.0F)
                {
                    n.u /= n.m;
                    n.v /= n.m;
                }
            }

            foreach (var p in _particles)
            {
                float gu = 0.0F;
                float gv = 0.0F;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Node n = _grid[(p.cx + i), (p.cy + j)];
                        float phi = p.px[i]*p.py[j];
                        gu += phi*n.u;
                        gv += phi*n.v;
                    }
                }
                p.x += gu;
                p.y += gv;
                p.u += 1.0F*(gu - p.u);
                p.v += 1.0F*(gv - p.v);
                if (p.x < 1.0F)
                {
                    p.x = (1.0F + (float) _rand.NextDouble()*0.01F);
                    p.u = 0.0F;
                }
                else if (p.x > GsizeX - 2)
                {
                    p.x = (GsizeX - 2 - (float) _rand.NextDouble()*0.01F);
                    p.u = 0.0F;
                }
                if (p.y < 1.0F)
                {
                    p.y = (1.0F + (float) _rand.NextDouble()*0.01F);
                    p.v = 0.0F;
                }
                else if (p.y > GsizeY - 2)
                {
                    p.y = (GsizeY - 2 - (float) _rand.NextDouble()*0.01F);
                    p.v = 0.0F;
                }
            }
        }

        public void MouseDragged(int x, int y)
        {
            _pressed = true;
            _mx = x;
            _my = y;
        }

        public void MouseMoved(int x, int y)
        {
            _mx = x;
            _my = y;
        }

        public void MousePressed()
        {
            _pressed = true;
        }

        public void MouseReleased()
        {
            _pressed = false;
        }
    
    }
}

//EOF
