using BS;
using UnityEngine;
namespace ThirdPersonCam
{
public class ItemTPCamera : MonoBehaviour {  
        protected Item item;
        TrailRenderer trail;Camera camera;
        bool start ; public ItemModuleTPCamera module;
        GameObject newgo = new GameObject(); Transform parentorg;int mode = 0;
        protected void Awake()
        {
            item = this.GetComponent<Item>();
            item.OnHeldActionEvent += OnHeldAction;
            module = item.data.GetModule<ItemModuleTPCamera>();
            start = false;mode = 0;
             parentorg = base.gameObject.transform.parent;
            if (module.trail_on)
            {
                trail = Player.local.gameObject.AddComponent<TrailRenderer>();
                trail.startColor = Color.white;
                trail.startWidth = 0.01f;
            }
        }

        public void OnHeldAction(Interactor interactor, Handle handle, Interactable.Action action)
        {
 
            if (!start && action == Interactable.Action.Ungrab)
            {

            start = !start;

                foreach (Transform child in item.gameObject.transform)
                {

                    if (child.name == "Colliders")
                    {

                        child.gameObject.SetActive(false);
                 
                    }
                }
            }
            if (action == Interactable.Action.AlternateUseStart)
            {
                switch (mode)
                {
                    case 0:
                        module.fixed_cam = true; 
                        module.force3p = true;
                        break;
                    case 1:
                        module.fixed_cam = true;
                        module.force3p = false;
                        break;
                    case 2:
                        module.fixed_cam = false; 
                        module.force3p = false; 
                        break;
                    default:
                        break;
                }
                mode = (mode + 1) % 2;
                       }

        }

        void FixedUpdate()
        {
            
            if (start)
            {
                if (camera == null)
                {
                    if (!module.fixed_cam)
                    {
                        base.gameObject.transform.position = new Vector3(0, 0, 0); ;
                        base.gameObject.transform.rotation = Quaternion.identity;
                   
                       if(Vector3.Dot(Player.local.locomotion.velocity, Player.local.head.transform.forward) < 0 || module.og_mode && (Vector3.Dot(Player.local.locomotion.velocity, Player.local.transform.eulerAngles) < 0))
                            newgo.transform.position = Player.local.transform.position + Player.local.locomotion.velocity.normalized * module.camera_distance;
                       else
                            newgo.transform.position = Player.local.transform.position - Player.local.locomotion.velocity.normalized * module.camera_distance;

                        newgo.transform.rotation = Player.local.transform.rotation;
                    }
                    else
                    {
                        base.gameObject.transform.position = Player.local.transform.position;
                        base.gameObject.transform.rotation = Player.local.transform.rotation;
                    }

                    camera = base.gameObject.AddComponent<Camera>();

                    if (!module.fixed_cam)
                        base.gameObject.transform.parent = newgo.transform;

                }

                if (Player.local.locomotion.velocity.magnitude > 0.5f || !Player.local.locomotion.isGrounded)
                {
                 //   Debug.Log("v: "+ Player.local.locomotion.velocity+ "r: " + Player.local.head.transform.forward);
                }
                else if(!module.force3p)
                {
                    Destroy(camera);
                    camera = null; 
                }

            }
        }
    }
}
