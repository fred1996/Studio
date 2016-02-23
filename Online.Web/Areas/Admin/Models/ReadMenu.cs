using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Online.Web.Areas.Admin.Models
{
    public class ReadMenu
    {
        public XmlDocument Root;
        private static string path = System.Web.HttpContext.Current.Server.MapPath("~/Areas/Admin/Content/Menu.xml");
        private static ReadMenu _instance = new ReadMenu();
        public List<Menus> Menues;
        private ReadMenu()
        {
            Root = new XmlDocument();
            Root.Load(path);
            Menues = new List<Menus>();
        }

        public static ReadMenu Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="Permissions">用户权限集合</param>
        public void GetMenu(List<string> Permissions)
        {
            Menues.Clear();
            XmlNode xn = Root.SelectSingleNode("Runtor.CRM");
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlElement item in xnl)
            {
                var menus = new Menus();
                menus.MenuName = item.GetAttribute("name");
                menus.Icon = item.GetAttribute("Icon");
                menus.Id = item.GetAttribute("id");
                menus.Menuses = new List<Menus>();
                foreach (XmlElement item1 in item.ChildNodes)
                {
                    var child = new Menus()
                    {
                        MenuName = item1.GetAttribute("name"),
                        Url = item1.GetAttribute("url")
                    };
                    if (Permissions.Count > 0)
                    {
                        if (Permissions.Contains(child.MenuName))
                        {
                            menus.Menuses.Add(child);
                        }
                    }
                    else
                    {
                        menus.Menuses.Add(child);
                    }
                   
                }
                if (menus.Menuses.Count > 0)
                {
                    Menues.Add(menus);
                }
            }
        }

    }

    public class Menus
    {
        public string MenuName { get; set; }

        public List<Menus> Menuses { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Id { get; set; }
    }
}