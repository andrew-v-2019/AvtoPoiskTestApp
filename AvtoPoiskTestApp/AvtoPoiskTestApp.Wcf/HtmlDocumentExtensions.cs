﻿using System;
using System.Reflection;
using System.Windows.Controls;
using mshtml;

namespace AvtoPoiskTestApp.Wcf
{
    public static class HtmlDocumentExtensions
    {
        public static void SetValueToElementById(this HTMLDocument doc, string id, string value)
        {
            var el = doc?.getElementById(id);
            el?.setAttribute("value", value);
        }

        public static void ClickElementWithId(this HTMLDocument doc, string id)
        {
            var el = doc?.getElementById(id);
            el?.click();
        }

        public static void RemoveElementWithId(this HTMLDocument doc, string id)
        {
            var el = doc?.getElementById(id);
            if (el != null && el.innerText != null)
            {
                el.innerText = string.Empty;
            }
        }

        public static bool IsLoginPage(this HTMLDocument doc)
        {
            var el = doc?.getElementById("password");
            return el != null;
        }

        public static bool IsRusLanguage(this HTMLDocument doc)
        {
            var el = doc?.getElementById("libelleflag");
            return el == null || el.parentElement.innerText.ToLower().Contains("русский");
        }

        public static bool IsInPage(this HTMLDocument doc, string uri)
        {
            if (doc == null)
            {
                return false;
            }

            var result = doc.url.ToLower().Contains(uri.ToLower());

            return result;
        }


        public static void HideJsScriptErrors(this WebBrowser wb)
        {
            var fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld == null)
                return;
            var obj = fld.GetValue(wb);
            obj?.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] {true});
        }

    }
}
