using UnityEditor;


[InitializeOnLoad]
public class GlobalConfig
{
    static GlobalConfig()
    {
        PlayerSettings.Android.keystorePass = "blockpuzzle2020";
        PlayerSettings.Android.keyaliasPass = "blockpuzzle2020";
        PlayerSettings.Android.keyaliasName = "blockpuzzle2020";
        PlayerSettings.SplashScreen.showUnityLogo = false;
    }
}