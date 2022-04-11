﻿using AntDesign;
using ImageBed.Data.Entity;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    partial class Index
    {
        List<UploadFileItem> fileList = new List<UploadFileItem>();
        Dictionary<string, object> attrs = new Dictionary<string, object>
    {
        {"accept", "image/*"},
        {"Action", "/api/image" },
        {"Name", "files" },
        {"Multiple", true }
    };

        int image_total = 0;                        // 待上传的图片总数
        int image_uploaded = 0;                     // 上传完成的图片数

        int _progress_percent = 0;                  // 上传完成的图片的百分比
        int progress_percent
        {
            get
            {
                return _progress_percent;
            }
            set
            {
                if (value == 0)
                {
                    progress_stroke_width = 0;
                    progress_showInfo = false;
                }
                else
                {
                    progress_stroke_width = 10;
                    progress_showInfo = true;
                }
                _progress_percent = value;
            }
        }
        int progress_stroke_width = 0;              // 进度条宽度
        bool progress_showInfo = false;             // 是否显示进度条信息


        void UpdateProgress()
        {
            progress_percent = (int)(image_uploaded * 100.0 / image_total);
        }


        bool uploadRunning = false;
        void CheckImages(UploadInfo imageInfo)
        {
            if (!uploadRunning)
            {
                image_total = fileList.Count;
                image_uploaded = 0;
                UpdateProgress();
                uploadRunning = true;
            }
        }


        void UploadImage(UploadInfo fileinfo)
        {
            image_uploaded = fileList.Where(file => file.State == UploadState.Success && !string.IsNullOrWhiteSpace(file.Response)).Count();
            UpdateProgress();
        }

        async void UploadFinished()
        {
            progress_percent = 0;

            // 获取图片链接
            string urls = string.Empty;
            fileList.ForEach(file =>
            {
                string url = file.GetResponse<ApiResult<List<string>>>()?.Res?.First() ?? string.Empty;
                if (url != string.Empty)
                {
                    if (file == fileList.Last())
                    {
                        urls += $"{NavigationManager.BaseUri}{url}";
                    }
                    else
                    {
                        urls += $"{NavigationManager.BaseUri}{url}\n";
                    }
                }
            });
            fileList.Clear();
            await JS.InvokeVoidAsync("CopyToClip", urls);
            _ = _message.Success("图片上传完成！");
        }
    }
}