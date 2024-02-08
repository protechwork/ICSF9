using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICSF9.Models
{
    public class general
    {
    }
    class Root
    {
        public List<Dictionary<string, object>> Data { get; set; }
    }
    class EInv
    {
        public int Roll { get; set; }
        public string name { get; set; }
        public List<string> courses { get; set; }
    }
    public class Footer
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public double Input { get; set; }
        public double Value { get; set; }
        public int ColMap { get; set; }
        public int Account1__Id { get; set; }
        public string Account1__Name { get; set; }
        public string Account1__Code { get; set; }
        public string Account1__Alias { get; set; }
        public int Account2__Id { get; set; }
        public string Account2__Name { get; set; }
        public string Account2__Code { get; set; }
        public string Account2__Alias { get; set; }
    }
    public class Response
    {
        public string url { get; set; }
        public List<CmpInfo> data { get; set; }
    }
    public static class StaticItems
    {
        public static string EndPoint = "http://localhost/Focus8API/";
    }
    public class CmpInfo
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
    }
    public class HashData
    {
        public List<Header> head { get; set; }
        public List<System.Collections.Hashtable> data { get; set; }
    }
    public class Header
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}