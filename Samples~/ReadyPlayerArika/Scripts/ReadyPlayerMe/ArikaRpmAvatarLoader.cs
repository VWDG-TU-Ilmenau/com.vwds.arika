using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace com.vwds.arika.readyplayerme
{
    /// <summary>
    /// This class is a simple <see cref="Monobehaviour"/>  to load Ready Player Me avatars and spawn as a <see cref="GameObject"/> into the scene.
    /// </summary>
    public class ArikaRpmAvatarLoader : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        [Tooltip("Set this to the URL or shortcode of the Ready Player Me Avatar you want to load.")]
        private string avatarUrl = "https://api.readyplayer.me/v1/avatars/638df693d72bffc6fa17943c.glb";
        private GameObject avatar;
        private void Awake()
        {
            GetComponent<Move>().enabled = false;
        }
        private void Start()
        {
            LoadAvatar(avatarUrl);
        }
        public void LoadAvatar(string avatarUrl)
        {
            this.avatarUrl = avatarUrl;
            var avatarLoader = new AvatarObjectLoader();
            avatarLoader.OnCompleted += (_, args) =>
            {
                avatar = args.Avatar;
                avatar.transform.parent = transform;
                GetComponent<Animator>().avatar = avatar.GetComponent<Animator>().avatar;
                GetComponent<Move>().AvatarFace = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).gameObject;
                GetComponent<Move>().enabled = true;
                Destroy(avatar.GetComponent<Animator>());
            };
            avatarLoader.LoadAvatar(avatarUrl);
        }
        private void OnDestroy()
        {
            if (avatar != null) Destroy(avatar);
        }
    }
}