namespace SDK.Proto
{
    public interface IProto
    {
        // manual implements this for performance
        // or use reflection for generic proto size
        public int ProtoSize { get; }
    }
}