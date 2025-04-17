namespace IMitto.Protocols.V1
{
	internal class MittoPackageBuilder : MittoPackageBuilderBase
	{
		protected override MittoProtocolVersion Version => MittoProtocolVersion.V1;

		public override IMittoPackage Build()
		{
			if (Action == MittoAction.None)
			{
				throw new InvalidOperationException("Action is not set.");
			}
			var command = new MittoCommand(Version, Action, Modifier);
			var headers = new MittoHeaders(Headers.Select(kvp => new MittoHeader(kvp.Key, kvp.Value)));
			var content = new MittoContent(Package);
			return new MittoPackage(command, headers, content);
		}
	}
}