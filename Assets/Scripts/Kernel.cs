public class Kernel : MarioObject
{
	public Flow.IKernel Kern;

	protected override void Construct()
	{
		base.Construct();

		Kern = Flow.Create.NewKernel();

		Kern.Root.Name = "Root";
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	protected override void Tick()
	{
		base.Tick();

		Kern.Step();
	}
}