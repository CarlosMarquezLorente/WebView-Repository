/*
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Collections;
using UnityEngine;
#if UNITY_2018_4_OR_NEWER
using UnityEngine.Networking;
#endif
using UnityEngine.UI;

public class SampleWebView : MonoBehaviour
{
    public string Url;
    public Text status;
    public Text statusError;
    WebViewObject webViewObject;
    public Image image;

    public void StartWebViewCoroutine()
    {
        StartCoroutine(CR_WebView());
    }

    IEnumerator CR_WebView()
    {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
                status.text += '\n' + msg;

                if (msg == "EXIT")
                {
                    //TODO: close popup webview
                }
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
                statusError.text += '\n' + msg;
            },
            httpErr: (msg) =>
            {
                Debug.Log(string.Format("CallOnHttpError[{0}]", msg));
                status.text += '\n' + msg;
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            hooked: (msg) =>
            {
                Debug.Log(string.Format("CallOnHooked[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                webViewObject.EvaluateJS(@"
                if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                window.setToken = {
                    call: function(msg) {
                    window.setToken('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
                    eyJpZCI6IjYyMjA4Y2Q0OGNhNzViMD
                    U2MjlkMjRkOSIsInVzZXJOYW1lIjoi
                    amF2aXBva2VyMDA3IiwiaWF0IjoxNj
                    UyMTc5NzcxLCJleHAiOjE2NTIxODMz
                    NzF9.
                    IeEhSIFWySfz6BeVjO5ioie2EgP_
                    sxq9-emElrqZfCw');
                    }
                  }
                }");
                webViewObject.EvaluateJS(@"
                if (window) {
                    window.setToken('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
                    eyJpZCI6IjYyMjA4Y2Q0OGNhNzViMD
                    U2MjlkMjRkOSIsInVzZXJOYW1lIjoi
                    amF2aXBva2VyMDA3IiwiaWF0IjoxNj
                    UyMTc5NzcxLCJleHAiOjE2NTIxODMz
                    NzF9.
                    IeEhSIFWySfz6BeVjO5ioie2EgP_
                    sxq9-emElrqZfCw');
                }");

            }
            //transparent: false,
            //zoom: true,
            //ua: "custom user agent string",
            //// android
            //androidForceDarkMode: 0,  // 0: follow system setting, 1: force dark off, 2: force dark on
            //// ios
            //enableWKWebView: true,
            //wkContentMode: 0,  // 0: recommended, 1: mobile, 2: desktop
            //wkAllowsLinkPreview: true,
            //// editor
            //separated: false
            );
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif
        // cf. https://github.com/gree/unity-webview/pull/512
        // Added alertDialogEnabled flag to enable/disable alert/confirm/prompt dialogs. by KojiNakamaru · Pull Request #512 · gree/unity-webview
        //webViewObject.SetAlertDialogEnabled(false);

        // cf. https://github.com/gree/unity-webview/pull/728
        //webViewObject.SetCameraAccess(true);
        //webViewObject.SetMicrophoneAccess(true);

        // cf. https://github.com/gree/unity-webview/pull/550
        // introduced SetURLPattern(..., hookPattern). by KojiNakamaru · Pull Request #550 · gree/unity-webview
        //webViewObject.SetURLPattern("", "^https://.*youtube.com", "^https://.*google.com");

        // cf. https://github.com/gree/unity-webview/pull/570
        // Add BASIC authentication feature (Android and iOS with WKWebView only) by takeh1k0 · Pull Request #570 · gree/unity-webview
        //webViewObject.SetBasicAuthInfo("id", "password");

        //webViewObject.SetScrollbarsVisibility(true);
   
        // webViewObject.SetMargins(0, 0, (int)image.rectTransform.rect.width, (int)image.rectTransform.rect.height);
        webViewObject.SetTextZoom(200);  // android only. cf. https://stackoverflow.com/questions/21647641/android-webview-set-font-size-system-default/47017410#47017410
        webViewObject.SetVisibility(true);
        

        webViewObject.LoadURL(Url);

        yield break;
    }

/*
    void OnGUI()
    {
        var x = 10;

        GUI.enabled = webViewObject.CanGoBack();
        if (GUI.Button(new Rect(x, 10, 80, 80), "<")) {
            webViewObject.GoBack();
        }
        GUI.enabled = true;
        x += 90;

        GUI.enabled = webViewObject.CanGoForward();
        if (GUI.Button(new Rect(x, 10, 80, 80), ">")) {
            webViewObject.GoForward();
        }
        GUI.enabled = true;
        x += 90;

        if (GUI.Button(new Rect(x, 10, 80, 80), "r")) {
            webViewObject.Reload();
        }
        x += 90;

        GUI.TextField(new Rect(x, 10, 180, 80), "" + webViewObject.Progress());
        x += 190;

        if (GUI.Button(new Rect(x, 10, 80, 80), "*")) {
            var g = GameObject.Find("WebViewObject");
            if (g != null) {
                Destroy(g);
            } else {
                StartCoroutine(Start());
            }
        }
        x += 90;

        if (GUI.Button(new Rect(x, 10, 80, 80), "c")) {
            Debug.Log(webViewObject.GetCookies(Url));
        }
        x += 90;

        if (GUI.Button(new Rect(x, 10, 80, 80), "x")) {
            webViewObject.ClearCookies();
        }
        x += 90;

        if (GUI.Button(new Rect(x, 10, 80, 80), "D")) {
            webViewObject.SetInteractionEnabled(false);
        }
        x += 90;

        if (GUI.Button(new Rect(x, 10, 80, 80), "E")) {
            webViewObject.SetInteractionEnabled(true);
        }
        x += 90;
    }
    */
}
