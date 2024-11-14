using ModelReplacement;
using UnityEngine;

namespace ModelReplacement
{
    public class MROWPRISONER : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        { 
            string model_name = "ImprovedPrisoner";
            return Assets.MainAssetBundle.LoadAsset<GameObject>(model_name);
        }
    }
}