using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using System;

public class GPS_System : MonoBehaviour
{
    [Header("UI")]
    //public Text gpsNotice;    
    public GameObject mainCanvas;
    public GameObject ImageBundle;
    public Text describeText;

    //public GameObject CityButtons;
    public GameObject BackButton;
    //public GameObject Ansan;
    //public GameObject Buyeo;
    //public GameObject Chang_neong;
    //public GameObject Chun_chen;
    public string AppID = "";
    public string strEdu_type = null;
    public string strSpot1 = null;
    public string strSpot2 = null;
    public string strSpot3 = null;
    public string strSpot4 = null;

    private Text[] aAnsanSpotsDistance = new Text[4];
    private double[] aDistance = new double[4];
    
    private float latitude;
    private float longitude;
    private int iCount = 0;
    private string gpsText = "";    
    private int iStampCount = 0;
    private int iGPS_PosCount = 0;

    private double[] aSpotLatitude = new double[4];
    private double[] aSpotLongitude  = new double[4];
    private GameObject[] aStampImg = new GameObject[4];
    private GameObject[] aButtonImg = new GameObject[4];
    private GameObject[] aImageBundle = new GameObject[4];
    private int iTestCount = 0;
    private int iButtonIndex = -1;
    private int iArriveIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < aStampImg.Length; ++i)
        {
            aStampImg[i] = mainCanvas.transform.GetChild(i + 1).gameObject.transform.GetChild(0).gameObject;
            aButtonImg[i] = mainCanvas.transform.GetChild(i + 1).gameObject.transform.GetChild(1).gameObject;
            aAnsanSpotsDistance[i] = mainCanvas.transform.GetChild(i + 1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
            aImageBundle[i] = ImageBundle.transform.GetChild(i).gameObject;
            aImageBundle[i].SetActive(false);
        }

        //모바일 location 정보에 대한 권한 설정...
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }            
        }
        ReStartGPS();
        //RequestGPS_Pos();
        //StampStatusUpdate();
    }

    // Update is called once per frame
    void Update()    {      }

    //================================================= GPS 정보 받기(경도, 위도) ==================================================//
    // GPS 정보 받기...    
    public void RequestGPS_Pos()
    {
        string structure = "";
        if (0 == iGPS_PosCount)
        {
            structure = strSpot1;
        }
        else if (1 == iGPS_PosCount)
        {
            structure = strSpot2;
        }
        else if (2 == iGPS_PosCount)
        {
            structure = strSpot3;
        }
        else if (3 == iGPS_PosCount)
        {
            structure = strSpot4;
        }
        else if (4 == iGPS_PosCount)
        {            
            StampCheck();            
            return;
        }

        //Debug.Log("check");
        DatabaseManager.Instance.GetGPS_Info(structure);
    }
    public void GetGPS_Pos(string _strAnswer)
    {
        //Debug.Log("answer: " + _strAnswer);
        string strAnswer = _strAnswer;
        if (strAnswer != "")
        {
            int location = strAnswer.IndexOf(",");
            int length = strAnswer.Length;
            int minusLength = length - location;
            string strLatitude = strAnswer.Substring(0, location);
            string strLongitute = strAnswer.Substring(location + 1, minusLength - 1);
            aSpotLatitude[iGPS_PosCount] = double.Parse(strLatitude);
            aSpotLongitude[iGPS_PosCount] = double.Parse(strLongitute);            
            
            // 계속 반복한다.
            Debug.Log("lati: " + aSpotLatitude[iGPS_PosCount]);
            Debug.Log("long: " + aSpotLongitude[iGPS_PosCount]);

            iGPS_PosCount++;
            RequestGPS_Pos();
        }
        else
        {
            // 에러....
        }
    }

    //====================================================================================================================//
    // 스탬프 획득 여부...
    void StampCheck()
    {
        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";
        string edu_type = strEdu_type + "_live";
        string game_type = "AR";
        
        if (0 == iStampCount)
        {
            kind = strEdu_type + "_sticker_first";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if(1 == iStampCount)
        {
            kind = strEdu_type + "_sticker_second";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if (2 == iStampCount)
        {
            kind = strEdu_type + "_sticker_third";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if (3 == iStampCount)
        {
            kind = strEdu_type + "_sticker_fourth";
            DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);
        }
        else if (4 == iStampCount)
        {
            StartCoroutine(GPS_KeepUpdate());
            return;
        }        
    }



    public void CatchStampInfo(string _strAnswer)
    {
        Debug.Log("answer: " + _strAnswer);
        string strAnswer = _strAnswer;
        if ("Y" == strAnswer)
        {
            // 이미 획득한 Stamp..            
            aStampImg[iStampCount].SetActive(true);
            aButtonImg[iStampCount].SetActive(false);
        }
        else
        {            
            // 획득 안한 Stamp...
            aStampImg[iStampCount].SetActive(false);
            aButtonImg[iStampCount].SetActive(true);
        }        
        iStampCount++;
        StampCheck();
    }
    






    //======================================================= Stamp 획득 ======================================================//  
    // 획득시가 아닌,,,, 
    public void StampStatusUpdate()
    {
        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";                
        string edu_type = strEdu_type + "_live";
        string game_type = "AR";
        int timeInfo = 0;

        if (0 == iStampCount)
        {
            // 수정 예정...
            get_flag = "Y";
            kind = strEdu_type + "_sticker_first";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (1 == iStampCount)
        {
            get_flag = "Y";
            kind = strEdu_type + "_sticker_second";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (2 == iStampCount)
        {
            get_flag = "Y";
            kind = strEdu_type + "_sticker_third";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (3 == iStampCount)
        {
            get_flag = "Y";
            kind = strEdu_type + "_sticker_fourth";
            DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
        }
        else if (4 == iStampCount)
        {
            ReStartGPS();
        }        
    }

    //==================================================== GPS INITIALIZE =====================================================//   
    void ReStartGPS()
    {

#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        PcFuncStart();
#elif UNITY_ANDROID
        StartCoroutine(GpsStart());        
#endif
        //StartCoroutine(GpsStart());
    }

    // 안드로이드 용..
    IEnumerator GpsStart()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            gpsText = "User has not enabled GPS";            
            describeText.text = gpsText;
            Invoke("ReStartGPS", 4f);
            yield break;
        }
        Input.location.Start(5, 10);
        int maxWait = 20;

        gpsText = Input.location.status.ToString();
        describeText.text = gpsText;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            //Input.location.status
            gpsText = Input.location.status.ToString();
            describeText.text = gpsText;
            maxWait--;
        }

        RequestGPS_Pos();

        while (true)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;            
            iCount++;            
            yield return new WaitForSeconds(0.5f);
        }
    }

    // pc용...
    void PcFuncStart()
    {        
        //latitude = (float)37.274092;
        //longitude = (float)126.839966;
        latitude = (float)37.484222;
        longitude = (float)126.899064;
        RequestGPS_Pos();
    }


    // 아래는 삭제 예정...
    // 아래는 삭제 예정...
    
    //================================================= GPS ===================================================//    

    IEnumerator GPS_KeepUpdate()
    {
        int iTmpCount = 0;

        while(true)
        {
            if (4 == iTmpCount)
                iTmpCount = 0;

            //Debug.Log("lati " + latitude);
            //Debug.Log("long " + longitude);

            if (false == aStampImg[iTmpCount].activeSelf)
            {                                
                // 모바일용...
                aDistance[iTmpCount] = GetDistance(latitude, longitude, aSpotLatitude[iTmpCount], aSpotLongitude[iTmpCount]);

                if (1000 <= aDistance[iTmpCount])                                                                            // 1000 미터 이상, 즉 kilo 로 표시...
                {
                    double dDist22 = aDistance[iTmpCount] / 1000;
                    string strDistance = dDist22.ToString("N1");
                    strDistance += "kilo";
                    aAnsanSpotsDistance[iTmpCount].text = strDistance;
                }
                else
                {
                    if (50 > aDistance[iTmpCount])                                                                              // 100 미터 이하일 경우 도착으로 표시...
                    {
                        // arrived...                
                        aAnsanSpotsDistance[iTmpCount].text = "";
                        // effect 가 있어야 할지도 있음....
                        aStampImg[iTmpCount].SetActive(true);
                        aButtonImg[iTmpCount].SetActive(false);

                        RequestCheckArrive(iTmpCount, latitude, longitude);
                    }
                    else
                    {
                        string strDistance = aDistance[iTmpCount].ToString("N0");
                        strDistance += "m";
                        aAnsanSpotsDistance[iTmpCount].text = strDistance;
                    }
                }
            }
            
            if (iButtonIndex == iTmpCount)
            {
                if (false == aStampImg[iTmpCount].activeSelf)
                {
                    describeText.text = aAnsanSpotsDistance[iTmpCount].text;
                }                
            }
                

            iTmpCount++;            
            yield return new WaitForSeconds(0.5f);
        }        
    }

    // 유저 id, 현재 gps, appID, 장소...
    void RequestCheckArrive(int _iTmeCount, double _latitude, double _longitute)
    {
        int iTmpCount = _iTmeCount;
        iArriveIndex = _iTmeCount;
        string dCurrentLatitude = _latitude.ToString();
        string dCurrentLongitude = _longitute.ToString();
        string dUserGPS = dCurrentLatitude + "," + dCurrentLongitude;

        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();

        string structure = "";
        if (0 == iTmpCount)
            structure = strSpot1;
        else if (1 == iTmpCount)
            structure = strSpot2;
        else if (2 == iTmpCount)
            structure = strSpot3;
        else if (3 == iTmpCount)
            structure = strSpot4;
        else if (4 <= iTmpCount)
            return;

        DatabaseManager.Instance.GPS_ArriveUpdate(appID, userID, structure, dUserGPS);
    }

    //DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
    public void StampStickerUpdate()
    {
        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";
        string edu_type = strEdu_type + "_live";
        string game_type = "AR";
        int timeInfo = 0;
        
        if (0 == iArriveIndex)
            kind = strEdu_type + "_sticker_first";
        else if (1 == iArriveIndex)
            kind = strEdu_type + "_sticker_second";
        else if (2 == iArriveIndex)
            kind = strEdu_type + "_sticker_third";
        else if (3 == iArriveIndex)
            kind = strEdu_type + "_sticker_fourth";
        else if (4 == iArriveIndex)
            return;

        get_flag = "Y";        
        // test 1 update...        
        DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);
    }

    public void CatchGpsUpdate(string _strAnswer)
    {
        Debug.Log("state: " + _strAnswer);
    }

    //==========================================================================================================//
    //================================================= BUTTON ===================================================//
    //========================================================================================================//
    public void SpotButtonEvent1()
    {
        for(int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[0].SetActive(true);

        if (false == aStampImg[0].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[0].text;
        }
        iButtonIndex = 0;
    }
    public void SpotButtonEvent2()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[1].SetActive(true);

        if (false == aStampImg[1].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[1].text;
        }
        iButtonIndex = 1;
    }
    public void SpotButtonEvent3()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[2].SetActive(true);

        if (false == aStampImg[2].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[2].text;
        }
        iButtonIndex = 2;
    }
    public void SpotButtonEvent4()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[3].SetActive(true);

        if (false == aStampImg[3].activeSelf)
        {
            describeText.text = aAnsanSpotsDistance[3].text;
        }
        iButtonIndex = 3;
    }

    //========================================================================================================================//
    public void StampButtonEvent1()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[0].SetActive(true);
        iButtonIndex = 0;

        describeText.text = "- 망우정은 어떤 곳인가요? 임진왜란 때의 의병장 곽재우(郭再祐 1552~1617)가 만년을 보냈던 곳이다. 곽재우는 1600년 봄에 병을 이유로 삼아 경상좌도병마절도사를 사직했는데 이 때문에 사헌부의 탄핵을 받아 2년 동안 전라도 영암으로 유배되었다. 그 후 현풍 비슬산에 들어가 은둔생활을 하다가 1602년 영산현 남쪽의 강가에 망우정(忘憂亭)을 짓고 기거하였다. - 망우정의 이름은 무슨 뜻인가요? 망우정은 ‘근심을 잊고 살겠다’는 뜻을 지닌 이름이다. 망우당(忘憂堂)이라는 곽재우의 호도 이로부터 비롯된 것이라 한다. 곽재우는 관직에 미련을 두지 않았는데 이는 이순신 장군의 투옥과, 절친했던 의병장 김덕령이 무고하게 옥에서 목숨을 잃은 사건에서 느낀 바가 있었기 때문으로 생각된다. 몇 차례 조정의 부름을 거절하지 못하고 관직에 나간 적이 있으나 결국 관직을 수차례 거절하고, 나룻배를 띄워 낚시를 하고, 거문고와 차를 가까이 하며 망우정에서 만년을 보냈다. - 망우정의 관람 포인트는 무엇인가요? 망우정은 굽이치는 낙동강과 강변 모래사장이 내려다보이는 야산 기슭에 자리 잡고 있다. 망우정은 3칸짜리 팔작지붕 기와집으로, 한국전쟁 때 소실되었다가 1972년 벽진이씨(碧珍李氏) 후손들이 중수하였고, 1979년 창녕군에서 전면 보수하였다. 건물 내부에 걸려 있는 ‘여현정기(餘賢亭記)’에 상세하게 기록되어 있다. 망우정 뒤쪽으로는 고을 유림들이 세운 충익공망우곽재우유허비(경상남도문화재자료 제23호)가 서 있다. [네이버 지식백과] 망우정[忘憂亭](두산백과)";
    }
    public void StampButtonEvent2()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[1].SetActive(true);
        iButtonIndex = 1;

        describeText.text = "만옥정은 창녕공원이라고도 한다. 면적 1만㎡의 규모가 작은 도시공원이지만, 지정문화재가 곳곳에 있으며, 봄철에 벚꽃이 핀 모습이 장관이어서 유명하다. 약 250년 전에 만옥정이라는 정자가 있었다고 하는데, 당시에는 봄마다 이곳에서 명창(名唱) 대회와 그네뛰기 대회가 열렸다고 전해진다.화왕산으로 가는 등산로가 옆으로 나 있다. 공원 뒤쪽 언덕 위에는 국내에서 가장 오래된 비인 신라진흥왕척경비(新羅眞興王拓境碑: 국보 33)가 있는데, 원래 창녕읍 화왕산록(火旺山麓)에 있던 것을 1924년에 이곳으로 옮겨온 것이다. 또한 공원에는 조선 후기의 관아건물인 창녕객사(경남유형문화재 231), 통일신라 후기에 만들어진 것으로 추정되는 토천 삼층석탑(兎川三層石塔: 경남유형문화재 10), 창녕척화비(경남문화재자료 218) 등이 있으며, 인근에 화왕산 군립공원, 창녕 교동고분군(사적 80), 창녕 석빙고(보물 310) 등의 문화재와 관광지가 자리잡고 있다. [네이버 지식백과] 만옥정공원[萬玉亭公園](두산백과)";
    }
    public void StampButtonEvent3()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[2].SetActive(true);
        iButtonIndex = 2;

        describeText.text = "관룡사는 신라 8대사찰의 하나로, 내물왕 39년인 394년에 창건되었다고 하나 확실하지는 않다. 583년(진평왕 5) 증법(證法)이 중창하고 삼국통일 후 원효가 이곳에서 중국 승려 1,000명에게 《화엄경》을 설법하여 대도량(大道埸)을 이루었다. 조선시대에 들어 태종 때에 대웅전을 중건하였으나 임진왜란 때 대부분 당우(堂宇)가 소실되어, 이후 재건과 보수 작업이 지속되었다. 관룡사에는 대웅전(보물 212호)과 약사전(藥師殿, 보물 146호)을 비롯하여 석조 여래좌상(石造如來坐像, 보물 519호)과 약사전 3층석탑(지방유형문화재 11호), 용선대(龍船臺) 석조 석가여래좌상(보물 295호) 등을 관람할 수 있다. [네이버 지식백과] 관룡사[觀龍寺] (두산백과)";
     }
    public void StampButtonEvent4()
    {
        for (int i = 0; i < aImageBundle.Length; ++i)
        {
            aImageBundle[i].SetActive(false);
        }
        aImageBundle[3].SetActive(true);
        iButtonIndex = 3;

        describeText.text = "국제습지조약 보존습지로 지정되어 국제적인 습지가 된 우포늪은 우포늪(1.3㎢), 목포늪(53만㎡), 사지포(36만㎡), 쪽지벌(14만㎡) 4개 늪으로 이루어져 있다. 1997년 342종의 동·식물이 이곳에 서식하는 것으로 조사·보고되었다. 식물은 가시연꽃·생이가래·부들·줄·골풀·창포·마름·자라풀 등 168종, 조류는 쇠물닭·논병아리·노랑부리저어새(천연기념물 205)·청둥오리·큰고니(천연기념물 201) 등 62종, 어류는 뱀장어·붕어·잉어·가물치·피라미 등 28종, 수서곤충은 연못하루살이·왕잠자리·장구애비·소금쟁이 등 55종, 패각류는 우렁이·물달팽이·말조개 등 5종, 포유류는 두더지·족제비· 너구리 등 12종, 파충류는 남생이·자라·줄장지뱀·유혈목이 등 7종, 양서류는 무당개구리·두꺼비·청개구리·참개구리·황소개구리 등 5종이 서식하고 있다. [네이버 지식백과] 우포늪[牛浦─](두산백과)";        
    }
    public void BackButtonEvent()
    {
        SceneManager.LoadScene("SelectMap");
    }
    public void TestButtonEvent()
    {
        if (4 <= iTestCount)
            return;
        aStampImg[iTestCount].SetActive(true);
        aButtonImg[iTestCount].SetActive(false);
        iTestCount++;
    }

    public void MapButtonEvent()
    {
        //https://www.google.com/maps/dir/37.483782,126.9003409/37.5081441,126.8385434        

        //iButtonIndex
        //aSpotLatitude , aSpotLongitude        
        if (-1 == iButtonIndex)
            return;

        //string strUrl = "https://www.google.com/maps/dir/" + latitude + "," + longitude + "/";
        //string strUrl = "https://www.google.com/maps/dir/" + latitude + "," + longitude + "/" + 37.94849 + "," + 127.8147;
        string strUrl = "https://www.google.com/maps/dir/" + latitude + "," + longitude + "/" + aSpotLatitude[iButtonIndex] + "," + aSpotLongitude[iButtonIndex];
        Application.OpenURL(strUrl);
    }


    public void TestButtonEvent2()
    {
        //37.66771 128.7053
        latitude = (float)37.66771;
        longitude = (float)128.7053;
    }


    // 지도 열기...
    // 아래와 같은 양식으로 한다.
    //https://www.google.com/maps/dir/37.483782,126.9003409/37.5081441,126.8385434



    //=================================================================================================================//
    //===================================================== EXTRA FUNCTION ===============================================//
    //=================================================================================================================//    
    // 거리 계산 Function...
    double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;
        double dLat1 = deg2rad(lat1);
        double dLat2 = deg2rad(lat2);
        double dTheta = deg2rad(theta);

        double dist = Math.Sin(dLat1) * Math.Sin(dLat2) + Math.Cos(dLat1) * Math.Cos(dLat2) * Math.Cos(dTheta);
        dist = Math.Acos(dist);
        double dDistResult = rad2deg(dist);

        dDistResult = dDistResult * 60 * 1.1515;        
        dDistResult = dDistResult * 1.6093344;
        dDistResult = dDistResult * 1000.0;
        return dDistResult;
    }
    // 방향 각도 구하기....
    public short BearingP1toP2(double P1_latitude, double P1_longitude, double P2_latitude, double P2_longitude)
    {
        // 현재 위치 : 위도나 경도는 지구 중심을 기반으로 하는 각도이기 때문에 라디안 각도로 변환한다.
        double cur_Lat_radian = P1_latitude * (3.141592 / 180);
        double cur_Lon_radian = P1_longitude * (3.141592 / 180);
        // 목표 위치 : 위도나 경도는 지구 중심을 기반으로 하는 각도이기 때문에 라디안 각도로 변환한다.
        double Dest_Lat_radian = P2_latitude * (3.141592 / 180);
        double Dest_Lon_radian = P2_longitude * (3.141592 / 180);
        
        // radian distance               
        //radian_distance = Mathf.Acos(Mathf.Sin(DoubleToFloat(cur_Lat_radian)) * Mathf.Sin(DoubleToFloat(Dest_Lat_radian)) + Mathf.Cos(DoubleToFloat(cur_Lat_radian)) * Mathf.Cos(DoubleToFloat(Dest_Lat_radian)) * Mathf.Cos(DoubleToFloat(cur_Lon_radian - Dest_Lon_radian)));
        double radian_distance = Math.Acos(Math.Sin(cur_Lat_radian)) * Math.Sin(Dest_Lat_radian) + Math.Cos(cur_Lat_radian) * Math.Cos(Dest_Lat_radian) * Math.Cos(cur_Lon_radian - Dest_Lon_radian);

        // 목적지 이동 방향을 구한다.(현재 좌표에서 다음 좌표로 이동하기 위해서는 방향을 설정해야 한다. 라디안값이다.
        //double radian_bearing = Mathf.Acos((Mathf.Sin(DoubleToFloat(Dest_Lat_radian)) - Mathf.Sin(DoubleToFloat(cur_Lat_radian)) * Mathf.Cos(DoubleToFloat(radian_distance))) / (Mathf.Cos(DoubleToFloat(cur_Lat_radian)) * Mathf.Sin(DoubleToFloat(radian_distance))));
        double radian_bearing = Math.Acos(Math.Sin(Dest_Lat_radian)) - Math.Sin(cur_Lat_radian) * Math.Cos(radian_distance) / Math.Cos(cur_Lat_radian) * Math.Sin(radian_distance);

        // acos의 인수로 주어지는 x는 360분법의 각도가 아닌 radian(호도)값이다.       
        double true_bearing = 0;
        if (Math.Sin(Dest_Lon_radian - cur_Lon_radian) < 0)
        {
            true_bearing = radian_bearing * (180 / 3.141592);
            true_bearing = 360 - true_bearing;
        }
        else
        {
            true_bearing = radian_bearing * (180 / 3.141592);
        }
        return (short)true_bearing;
    }
    static double deg2rad(double _deg)
    {
        return (_deg * Mathf.PI / 180d);
    }
    static double rad2deg(double _rad)
    {
        return (_rad * 180d / Mathf.PI);
    }


    //===================================================== USELESS ===============================================//


    void AnsanCalculateDistance(int _iCount)
    {
        int iTempCount = _iCount;

        // test 용...        
        //double dMyLatitude = 37.274092;
        //double dMyLogitude = 126.839966;
        //pc 테스트용...        
        //aDistance[iTempCount] = GetDistance(dMyLatitude, dMyLogitude, aSpotLatitude[iTempCount], aSpotLongitude[iTempCount]);
        // 모바일용...
        aDistance[iTempCount] = GetDistance(latitude, longitude, aSpotLatitude[iTempCount], aSpotLongitude[iTempCount]);

        if (1000 <= aDistance[iTempCount])                                                                            // 1000 미터 이상, 즉 kilo 로 표시...
        {
            if (true == aStampImg[iTempCount].activeSelf)
                return;

            double dDist22 = aDistance[iTempCount] / 1000;
            string strDistance = dDist22.ToString("N1");
            strDistance += "kilo";

            aAnsanSpotsDistance[iTempCount].text = strDistance;
        }
        else
        {
            if (true == aStampImg[iTempCount].activeSelf)
                return;

            if (50 > aDistance[iTempCount])                                                                              // 100 미터 이하일 경우 도착으로 표시...
            {
                // arrived...                
                aAnsanSpotsDistance[iTempCount].text = "";
                //aAnsanSpotsStampBg[iTempCount].SetActive(true);
                //aAnsanSpotsStamp[iTempCount].SetActive(true);

                // 그리고 db 에 send 한다....
                // 도착...
                //RequestCheckArrive(iTempCount, dMyLatitude, dMyLogitude);
                RequestCheckArrive(iTempCount, latitude, longitude);
            }
            else
            {
                string strDistance = aDistance[iTempCount].ToString("N0");
                strDistance += "m";
                aAnsanSpotsDistance[iTempCount].text = strDistance;
            }
        }
    }


    // 안산시 버튼을 클리시에....
    public void AnsanButtonEvent()
    {
        // 총 4개를 받아온다.
        // 여기서 시작해서,,,
        // 아래처럼.....

        //string appID = PlayerInfo.Instance.GetAppID();
        string appID = AppID;
        string userID = PlayerInfo.Instance.GetUserID();
        //string kind = "social_sticker_first";
        string kind = "";
        string get_flag = "";
        string edu_type = "social_live";
        string game_type = "AR";
        int timeInfo = 0;

        /*
        if (0 == iStampCount)
            kind = "social_sticker_first";
        else if (1 == iStampCount)
            kind = "social_sticker_second";
        else if (2 == iStampCount)
            kind = "social_sticker_third";
        else if (3 == iStampCount)
            kind = "social_sticker_fourth";
        else if (4 == iStampCount)
            return;
        */

        get_flag = "Y";
        //kind = "social_sticker_first";
        kind = "social_sticker_second";


        Debug.Log("app ID: " + appID);
        Debug.Log("user ID: " + userID);

        // 여기서 먼저 체크를 해야 한다.
        // first ~ fourth 까지 각각 받아온다.
        // 총 4개의 코루틴 함수를 받아온다.

        // db 에서 stamp 결과 받아오기....
        // 성공...
        //DatabaseManager.Instance.GetStamp(appID, userID, kind, edu_type);

        // test 1 update...
        // 성공...
        DatabaseManager.Instance.GpsInfoUpdate(appID, userID, kind, get_flag, edu_type, game_type, timeInfo);

        /*
        CityButtons.SetActive(false);        
        Buyeo.SetActive(false);
        Chang_neong.SetActive(false);
        Chun_chen.SetActive(false);
        Ansan.SetActive(true);
        BackButton.SetActive(true); 
        
        eCity = GPS_CITY.ANSAN;
        */
    }

    /*
    public void CatchGpsUpdate(string _strAnswer)
    {
        Debug.Log("answer: " + _strAnswer);
        string strAnswer = _strAnswer;
        if ("Y" == strAnswer)
        {
            // 이미 획득한 Stamp..
            Debug.Log("already have");
            aStampImg[iStampCount].SetActive(true);
            aButtonImg[iStampCount].SetActive(false);
        }
        else
        {
            // 획득 안한 Stamp...
            aStampImg[iStampCount].SetActive(false);
            aButtonImg[iStampCount].SetActive(true);
        }

        // 여기서 call....
        iStampCount++;
        Invoke("StampStatusUpdate", 0.2f);
    }
    */
}
