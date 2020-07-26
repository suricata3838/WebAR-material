using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace STYLY.Uploader
{
	public class Utility
	{
        public static bool IsUrl(string input)
        {
            try
            {
                var uri = new Uri(input);
                if (uri.Scheme != "http" && uri.Scheme != "https")
                {
                    return false;
                }

                return Uri.IsWellFormedUriString(input, UriKind.Absolute);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsVersionString(string version)
        {
            try
            {
                new System.Version(version);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}