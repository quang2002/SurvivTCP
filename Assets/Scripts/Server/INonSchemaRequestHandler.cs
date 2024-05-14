namespace Server
{
    public interface IRequestHandler<in T>
        where T : unmanaged
    {
        public void Handler(ClientInfo info, T packet);
    }

    public interface INonSchemaRequestHandler
    {
        public unsafe int Handler(ClientInfo info, byte* packet);
    }
}