﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KibotController
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class Device
    {
        private IConnect connect = null;

        public Device(IConnect connect)
        {
            this.connect = connect;
        }

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <returns></returns>
        public string[] DeviceList()
        {
            string log = connect.ExecuteAdb("devices", false);
            string[] lines = log.Trim().Split("\n".ToCharArray());
            string[] result = new string[lines.Length];
            int index = 1;
            foreach (string line in lines)
            {
                string l = line.Trim();
                if (l.StartsWith("List") || l.StartsWith("*"))
                {
                    continue;
                }
                string[] ds = l.Split("\t".ToCharArray());
                if (ds.Length > 0 && ds[0] != "")
                {
                    result[index++] = ds[0];
                }
            }
            if (index == 1)
            {
                result[0] = "没有设备";
            }
            else
            {
                result[0] = "请选择设备";
            }
            return result;
        }

        /// <summary>
        /// 获取当前设备的信息
        /// </summary>
        /// <returns></returns>
        public string[] GetCurrentDeviceInfo()
        {
            string log = connect.ExecuteAdb("devices -l", false);
            string[] lines = log.Split("\n".ToCharArray());
            string[] result = new string[4];
            foreach (string line in lines)
            {
                if (!line.Contains(connect.GetDeviceSerial()))
                {
                    continue;
                }
                Match match = Regex.Match(log, @"(.*)\sdevice product:(\w+)\smodel:(\w+)\sdevice:(\w+)\r");
                if (match.Groups.Count > 4)
                {
                    result[0] = match.Groups[1].Value;
                    result[1] = match.Groups[2].Value;
                    result[2] = match.Groups[3].Value;
                    result[3] = match.Groups[4].Value;
                    return result;
                }
            }
            return null;
        }
    }
}
