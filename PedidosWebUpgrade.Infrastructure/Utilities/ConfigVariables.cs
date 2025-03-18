namespace PedidosWebUpgrade.Infrastructure.Utilities
{
    public sealed class ConfigVariables
    {
        public required string ConnectionString { get; init; }
        public required string LogDirectory { get; init; }

        public required string RepoImg { get; init; }

        public required string PedidosPDF { get; init; }
    }
}
