using YooAsset;

namespace Launcher.Yoo
{
    public class DeliveryQueryServices : YooAsset.IDeliveryQueryServices
    {
        public bool QueryDeliveryFiles(string packageName, string fileName)
        {
            return false;
        }

        public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}