using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Nilsa
{
    struct WebBrowserCommand
    {
        public const uint GoToPersonePage = 0;
        public const uint RepostSetOfWallPostsToWall = 1;
        public const uint LikeContacterAva = 2;
        public const uint JoinCommunity = 3;
        public const uint RepostPostFromWallToWall = 4;
        public const uint RepostPostFromGroupToWall = 5;
        public const uint CheckPersonePage = 6;
        public const uint ShowFriendsPage = 7;
        public const uint Autorize = 8;
        public const uint LoginPersone = 9;
        public const uint GoToContactPage = 10;
        public const uint ShowContactPage = 11;
        public const uint SendMessage = 12;
        public const uint ReadMessages = 13;
        public const uint ReadHistory = 14;
        public const uint GetPhotoURL = 15;
        public const uint GetPersoneAttributes = 16;
        public const uint GetContactAttributes = 17;
        public const uint GetPersoneFriendsCount = 18;
        public const uint AddToFriends = 19;
        public const uint GetPersoneName = 20;

        public const uint None = 100;
        public const int Count = 20;
    }

    static class NilsaUtils
    {
        public static String TextToString(String Text)
        {
            return Text.Replace("\n", "<br>").Replace("\r", "").Replace('|', ' ').Trim();
        }

        public static String StringToText(String Text)
        {
            return Text.Replace("<br>", "\r\n").Trim();
        }

        public static void SaveStringValue(int valueid, String value)
        {
            if (valueid < 0)
                return;

            List<String> lstContHar = new List<String>();
            lstContHar.Add(value);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "StringValues" + Convert.ToString(valueid) + ".values"), lstContHar, Encoding.UTF8);
        }

        public static String LoadStringValue(int valueid, String defvalue)
        {
            if (valueid < 0)
                return defvalue;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "StringValues" + Convert.ToString(valueid) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "StringValues" + Convert.ToString(valueid) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
                return lstContHar[0];

            return defvalue;
        }


        public static void SaveRuCaptcha(String value)
        {
            List<String> lstContHar = new List<String>();
            lstContHar.Add(value);

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "ruCapthaApiKey.txt"), lstContHar, Encoding.UTF8);
        }

        public static void DeleteRuCaptcha()
        {
            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "ruCapthaApiKey.txt")))
                File.Delete(Path.Combine(Application.StartupPath, "ruCapthaApiKey.txt"));
        }

        public static String LoadRuCaptcha(String defvalue)
        {
            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "ruCapthaApiKey.txt")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "ruCapthaApiKey.txt"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
                if (lstContHar[0].Length>0)
                    return lstContHar[0];

            return defvalue;
        }

        public static void SaveLongValue(int valueid, long value)
        {
            if (valueid < 0)
                return;

            List<String> lstContHar = new List<String>();
            lstContHar.Add(value.ToString());

            if (lstContHar.Count > 0)
                File.WriteAllLines(Path.Combine(Application.StartupPath, "LongValues" + Convert.ToString(valueid) + ".values"), lstContHar, Encoding.UTF8);
        }

        public static long LoadLongValue(int valueid, long defvalue)
        {
            if (valueid < 0)
                return defvalue;

            List<String> lstContHar = new List<String>();
            if (File.Exists(Path.Combine(Application.StartupPath, "LongValues" + Convert.ToString(valueid) + ".values")))
            {
                var srcFile = File.ReadAllLines(Path.Combine(Application.StartupPath, "LongValues" + Convert.ToString(valueid) + ".values"));
                lstContHar = new List<String>(srcFile);
            }

            if (lstContHar.Count > 0)
                return Convert.ToInt64(lstContHar[0]);

            return defvalue;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static bool Dictonary_CheckControlEmptyTextValue(string text)
        {
            if (text == null)
                return false;
            if (text.Length == 0)
                return false;

            if (text.Trim().Length == 0)
                return false;

            if (text.Equals("0"))
                return false;
            if (text.Equals("..."))
                return false;
            if (text.Equals("00:00"))
                return false;
            if (text.Equals("00.00"))
                return false;
            if (text.StartsWith("Form"))
                return false;
            if (text.StartsWith("label"))
                return false;
            if (text.StartsWith("toolStrip"))
                return false;

            return true;
        }

        public static void Dictonary_AddItem(Dictionary<string, string> ControlList, string key, string value)
        {
            if (!ControlList.ContainsKey(key))
                ControlList.Add(key, value);
        }

        public const bool RELEASE_MODE = true;

        public static SolidBrush brushListBoxSelected = new SolidBrush(SystemColors.Highlight);
        public static SolidBrush brushListBoxHasValue = new SolidBrush(SystemColors.ActiveCaption);
        public static SolidBrush brushListBoxEmptyValue = new SolidBrush(SystemColors.Window);
        public static Color foreColorListBoxSelected = SystemColors.HighlightText;
        public static Color foreColorListBoxHasValue = SystemColors.ActiveCaptionText;
        public static Color foreColorListBoxEmptyValue = SystemColors.ControlText;

        public static void Dictonary_AddAllControlsText(Dictionary<string, string> ControlList, Control container, string name, ToolTip toolTipControl=null)
        {
            if (RELEASE_MODE)
                return;

            if (container is ComboBox)
                return;

            if (Dictonary_CheckControlEmptyTextValue(container.Text)) Dictonary_AddItem(ControlList, name + "." + container.Name, container.Text);

            if (container is ToolStrip)
            {
                ToolStrip ts = container as ToolStrip;
                foreach (ToolStripItem c in ts.Items)
                {
                    if (Dictonary_CheckControlEmptyTextValue(c.Text)) Dictonary_AddItem(ControlList, name + "." + c.Name, c.Text);
                    if (Dictonary_CheckControlEmptyTextValue(c.ToolTipText)) Dictonary_AddItem(ControlList, name + "." + c.Name + ".ToolTipText", c.ToolTipText);
                    //if (c.Text.Length>0) ControlList.Add(c);

                    if (c is ToolStripSplitButton)
                    {
                        foreach (ToolStripItem ddi in ((ToolStripSplitButton)c).DropDownItems)
                        {
                            if (Dictonary_CheckControlEmptyTextValue(ddi.Text)) Dictonary_AddItem(ControlList, name + "." + ddi.Name, ddi.Text);
                            if (Dictonary_CheckControlEmptyTextValue(ddi.ToolTipText)) Dictonary_AddItem(ControlList, name + "." + ddi.Name + ".ToolTipText", ddi.ToolTipText);
                            //if (c.Text.Length>0) ControlList.Add(c);
                        }
                    }
                }
            }
            else
            {
                if (toolTipControl != null)
                {
                    String tooltiptext = toolTipControl.GetToolTip(container);
                    if (Dictonary_CheckControlEmptyTextValue(tooltiptext)) Dictonary_AddItem(ControlList, name + "." + container.Name + ".ToolTipText", tooltiptext);
                }

                foreach (Control c in container.Controls)
                {
                    Dictonary_AddAllControlsText(ControlList, c, name/*+"."+ container.Name*/, toolTipControl);
                    //if (c.Text.Length>0) ControlList.Add(c);
                }
            }
        }

        public static string Dictonary_GetText(Dictionary<string, string> ControlList, string nameItem, string nameForm, string textDefault)
        {
            if (ControlList.ContainsKey(nameForm + "." + nameItem))
                return ControlList[nameForm + "." + nameItem];

            return textDefault;
        }

        public static void Dictonary_ApplyAllControlsText(Dictionary<string, string> ControlList, Control container, string name, ToolTip toolTipControl = null)
        {
            if (container is ComboBox)
                return;

            if (Dictonary_CheckControlEmptyTextValue(container.Text))
            {
                try
                {
                    if (ControlList.ContainsKey(name + "." + container.Name))
                        container.Text = ControlList[name + "." + container.Name];
                }
                catch { }
            }

            if (container is ToolStrip)
            {
                ToolStrip ts = container as ToolStrip;
                foreach (ToolStripItem c in ts.Items)
                {
                    if (Dictonary_CheckControlEmptyTextValue(c.Text))
                    {
                        try
                        {
                            if (ControlList.ContainsKey(name + "." + c.Name))
                                c.Text = ControlList[name + "." + c.Name];
                        }
                        catch { }
                    }
                    if (Dictonary_CheckControlEmptyTextValue(c.ToolTipText))
                    {
                        try
                        {
                            if (ControlList.ContainsKey(name + "." + c.Name + ".ToolTipText"))
                                c.ToolTipText = ControlList[name + "." + c.Name + ".ToolTipText"];
                            //else
                            //    c.ToolTipText = c.Text;
                        }
                        catch { }
                    }
                    //else
                    //{
                    //    c.ToolTipText = c.Text;
                    //}

                    if (c is ToolStripSplitButton)
                    {
                        foreach (ToolStripItem ddi in ((ToolStripSplitButton)c).DropDownItems)
                        {
                            if (Dictonary_CheckControlEmptyTextValue(ddi.Text))
                            {
                                try
                                {
                                    if (ControlList.ContainsKey(name + "." + ddi.Name))
                                        ddi.Text = ControlList[name + "." + ddi.Name];
                                }
                                catch { }
                            }
                            if (Dictonary_CheckControlEmptyTextValue(ddi.ToolTipText))
                            {
                                try
                                {
                                    if (ControlList.ContainsKey(name + "." + ddi.Name + ".ToolTipText"))
                                        ddi.ToolTipText = ControlList[name + "." + ddi.Name + ".ToolTipText"];
                                    //else
                                    //    ddi.ToolTipText = ddi.Text;
                                }
                                catch { }
                            }
                        }
                    }

                    //if (c.Text.Length>0) ControlList.Add(c);
                }
            }
            else
            {
                if (toolTipControl!=null)
                {
                    try
                    {
                        if (ControlList.ContainsKey(name + "." + container.Name+ ".ToolTipText"))
                            toolTipControl.SetToolTip(container, ControlList[name + "." + container.Name + ".ToolTipText"]);
                    }
                    catch { }
                }

                foreach (Control c in container.Controls)
                {
                    Dictonary_ApplyAllControlsText(ControlList, c, name/*+"."+ container.Name*/, toolTipControl);
                    //if (c.Text.Length>0) ControlList.Add(c);
                }
            }
        }

        public static void Dictionary_Save(IDictionary<string, string> d, string filename)
        {
            if (RELEASE_MODE)
                return;

            new XElement("root", d.Select(kv => new XElement(kv.Key, kv.Value)))
                        .Save(filename, SaveOptions.OmitDuplicateNamespaces);
        }
        public static Dictionary<string, string> Dictionary_Load(string filename)
        {
            return XElement.Parse(File.ReadAllText(filename))
                           .Elements()
                           .ToDictionary(k => k.Name.ToString(), v => v.Value.ToString());
        }
    }
}
