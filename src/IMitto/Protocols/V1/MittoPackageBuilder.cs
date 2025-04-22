namespace IMitto.Protocols.V1
{
	internal sealed class MittoPackageBuilder : MittoPackageBuilderBase
	{
		protected override MittoProtocolVersion Version => MittoProtocolVersion.V1;

		public override IMittoPackage Build()
		{
			if (Action == MittoAction.None)
			{
				throw new InvalidOperationException("Action is not set.");
			}
			
			var command = new MittoCommand(Action, Modifier);
			return new MittoPackage(command, new MittoHeaders(Headers), new MittoContent(Package));
		}

		protected override IMittoHeader CreateHeader(string key, string value)
		{
			return new MittoHeader(key, value);
		}
	}
}