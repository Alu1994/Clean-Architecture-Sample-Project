using CleanArchitectureSampleProject.Presentation.gRPC2;

namespace CleanArchitectureSampleProject.Presentation.Web;

public static class Extensions
{
    public static ProtoGuid ToGrpcGuid(this Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        ulong[] longs = new ulong[2];
        longs[0] = BitConverter.ToUInt64(bytes, 0);
        longs[1] = BitConverter.ToUInt64(bytes, 8);

        return new ProtoGuid()
        {
            Lo = longs[0],
            Hi = longs[1]
        };
    }

    public static Guid ToSystemGuid(this ProtoGuid guidGrpc)
    {
        ulong low = Convert.ToUInt64(guidGrpc.Lo.ToString());
        ulong high = Convert.ToUInt64(guidGrpc.Hi.ToString());

        uint a = (uint)(low >> 32), b = (uint)low, c = (uint)(high >> 32), d = (uint)high;
        return new Guid((int)b, (short)a, (short)(a >> 16),
                        (byte)d, (byte)(d >> 8), (byte)(d >> 16), (byte)(d >> 24),
                        (byte)c, (byte)(c >> 8), (byte)(c >> 16), (byte)(c >> 24));
    }

    public static decimal ToDecimal(this ProtoDecimal value)
    {
        return new decimal([value.V1, value.V2, value.V3, value.V4]);
    }

    public static ProtoDecimal ToProtoDecimal(this decimal value)
    {
        var bits = decimal.GetBits(value);
        return new ProtoDecimal
        {
            V1 = bits[0],
            V2 = bits[1],
            V3 = bits[2],
            V4 = bits[3]
        };
    }
}
