using System.Collections;
using System.Collections.Generic;
using UnityEngine;

{   
    public class DownloadHandler : Singleton<DownloadHandler>
    {
        private class DownloadItem
        {
            public string Url { get; set; }
            public Callback DownloadCallback { get; set; }
            public OnSuccess onSuccess;
            public OnFailed onFailed;

            public DownloadItem(string url, Callback dlcf, OnSuccess onSuccess, OnFailed onFailed)
            {
                this.Url = url;
                this.DownloadCallback = dlcf;
                this.onSuccess = onSuccess;
                this.onFailed = onFailed;
            }
        }

        protected DownloadHandler() {}

        private WWW wwwData;

        public delegate void OnSuccess(WWW www);
        public delegate void OnFailed(WWW www);
        public delegate void Callback(WWW www, OnSuccess onSuccess, OnFailed onFailed);

        private static Queue<DownloadItem> downloadQueue;

        public void Initialize()
        {
            downloadQueue = new Queue<DownloadItem>();

            StartCoroutine(UpdateDownloadQueue());
        }       

        public void Download(string url, Callback dlcf, OnSuccess onSuccess, OnFailed onFailed)
        {
            downloadQueue.Enqueue(new DownloadItem(url, dlcf, onSuccess, onFailed));
        }

        private IEnumerator UpdateDownloadQueue()
        {
            for(;;)
            {
                if (downloadQueue.Count == 0 || (wwwData != null && wwwData.isDone == false))
                    yield return new WaitForSeconds(0.25f);
                else
                     StartCoroutine(StartDownload());
            }
        }

        private IEnumerator StartDownload()
        {
            UIHandler.Instance.UpdateLoadingIconRotation(true);

            DownloadItem dlItem = downloadQueue.Dequeue();

            yield return StartCoroutine(OnDownload(dlItem));

            dlItem.DownloadCallback(wwwData, dlItem.onSuccess, dlItem.onFailed);

            UIHandler.Instance.UpdateLoadingIconRotation(false);
        }

        private IEnumerator OnDownload(DownloadItem dlItem)
        {
            this.wwwData = new WWW(dlItem.Url);

            yield return wwwData;
        }
    }
}
