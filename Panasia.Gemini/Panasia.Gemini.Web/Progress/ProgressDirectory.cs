using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.Web.Progress
{
    public static class ProgressDirectory
    {
        private static ConcurrentDictionary<string, FileProgress> upLoadProgress = new ConcurrentDictionary<string, FileProgress>();

        public static void PutValue(string key, FileProgress fileProgress)
        {
            try
            {
                if (upLoadProgress.ContainsKey(key))
                {
                    upLoadProgress[key] = fileProgress;
                }
                else
                {
                    upLoadProgress.TryAdd(key, fileProgress);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static FileProgress GetProgress(string key)
        {
            if (upLoadProgress.ContainsKey(key))
            {
                return upLoadProgress[key];
            }
            else
            {
                return new FileProgress();
            }
        }

        public static void Clear(string key)
        {
            FileProgress progress;
            try
            {
                if (upLoadProgress.ContainsKey(key))
                {
                    upLoadProgress.TryRemove(key, out progress);
                } 
            }
            catch (Exception ex)
            { }
        }
    }
}