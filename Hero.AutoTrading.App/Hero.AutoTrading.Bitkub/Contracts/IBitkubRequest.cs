
namespace Hero.AutoTrading.Bitkub
{
    public interface IBitkubRequest
    {
        int TimeStamp { get; }
        string Signature { get; }

        void SetTimeStampToCurrentTime();
        void UpdateSignature(string signature);
    }
}
