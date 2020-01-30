using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private const string ACCESS_FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
    private const string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
    private const string CAMERA = "android.permission.CAMERA";

    private static PlayerInfo instance = null;
    private static readonly object padlock = new object();

    private PlayerInfo()
    {

    }

    public static PlayerInfo Instance
    {
        get
        {
            lock(padlock)
            {
                if(instance==null)
                {
                    instance = new PlayerInfo();
                }
                return instance;
            }
        }
    }

    public List<int> GetMedal2D = new List<int>();
    public List<int> GetMedal3D = new List<int>();
        
    public bool isComplite;   
    public bool isClicked;
    public bool ClearQuiz;


    // 0 = 여자 // 1 = 남자
    public int Gender = 0;


    // 추가됨...
    private const string appID = "korean_AR";
    private string userID = "12345";

    // Start is called before the first frame update
    void Start()
    {
        if(instance!=null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        

        AndroidRuntimePermissions.Permission[] result = AndroidRuntimePermissions.RequestPermissions(WRITE_EXTERNAL_STORAGE, CAMERA, ACCESS_FINE_LOCATION);

        for(int i=0;i<result.Length;i++)
        {
            if(result[i]==AndroidRuntimePermissions.Permission.Granted)
            {
                Debug.Log("We have all the permissions!");
            }
            else
            {
                Debug.Log("Some permission are not granted...");
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }




    // 추가됨....
    public void SetUserID(string _userID) { userID = _userID; }
    public string GetUserID() { return userID; }
    public string GetAppID() { return appID; }
}
