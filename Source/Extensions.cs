using System;
using Multiplayer.API;

namespace Multiplayer.Compat
{
    internal static class Extensions
    {
        internal static string After(this string s, char c)
        {
            if (s.IndexOf(c) == -1)
                throw new Exception($"Char {c} not found in string {s}");
            return s.Substring(s.IndexOf(c) + 1);
        }

        internal static string Until(this string s, char c)
        {
            if (s.IndexOf(c) == -1)
                throw new Exception($"Char {c} not found in string {s}");
            return s.Substring(0, s.IndexOf(c));
        }

        internal static int CharacterCount(this string s, char c)
		{
			int num = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == c)
				{
					num++;
				}
			}
			return num;
		}

        internal static void SetDebugOnly(this ISyncMethod[] syncMethods)
        {
            foreach(var method in syncMethods)
            {
                method.SetDebugOnly();
            }
        }

        internal static void SetContext(this ISyncDelegate[] syncDelegates, SyncContext context)
        {
            foreach(var method in syncDelegates)
            {
                method.SetContext(context);
            }
        }
    }
}