using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nilsa
{
    public class AlgorithmsDBRecord
    {
        public int ID;
        public String Name;
        public int CompareLexicalLevel;
        public int CompareVectorLevel;
        public int[] iMsgHarKoef;
        public int[] iMsgHarTypes;
        public bool[] bMsgReplaceValue;
        public int[] iMarkerAlgorithmsID;
        public int[] iMarkerContHarID;
        public String[] sMarkerContHarValues;
        public int[] iMarkerMsgHarID;
        public String[] sMarkerMsgHarValues;
        public bool bLockDuplicateSend;
        public bool bFriendsGet;
        public bool bFriendsAdd;
        public bool bFriendsMarker;
        public bool bGroupingOutMessages;
        public bool bIgnoreMessagesFromNotContacter;
        public bool bNotGenerateUnknownMessages;
        public String DefaultAlgorithmTheme;
        public bool ChangeThemeAlgorithmINMessageTheme;
        public bool ChangeThemeAlgorithmNotStoreLastMsg;
        public bool IgnoreUnknownMessagesWOUnknownGeneration;
        public bool SplitTextIntoSentences;
        public String[] sMarkerInMessages;
        public bool LinkAdditionalThemes;
        public bool PlayReceiveMsg;
        public bool PlaySendMsg;
        public bool PlayMarker;
        public bool ShowErrorDetails;
        public String AllowedAdditionalThemesList;
        public bool MergeInMessages;
        public bool UseSetKoefWithLexicalCompare;
        public int CompareStarLevel;

        public AlgorithmsDBRecord()
        {
            new AlgorithmsDBRecord(-1, "", 0, 0, new int[0], new int[0], new bool[0], new int[0], new int[0], new String[0], new int[0], new String[0], true, true, true, true, true, false, false, "", true, true, false, true, new String[0], true, true, true, true, false,"",false, true, 90);
        }

        public AlgorithmsDBRecord(int _ID, String _Name, int _CompareLexicalLevel, int _CompareVectorLevel, int[] _iMsgHarKoef, int[] _iMsgHarTypes, bool[] _bMsgReplaceValue, int[] _iMarkerAlgorithmsID, int[] _iMarkerContHarID, String[] _sMarkerContHarValues, int[] _iMarkerMsgHarID, String[] _sMarkerMsgHarValues, bool _bLockDuplicateSend, bool _bFriendsGet, bool _bFriendsAdd, bool _bFriendsMarker, bool _bGroupingOutMessages, bool _bIgnoreMessagesFromNotContacter, bool _bNotGenerateUnknownMessages, String _DefaultAlgorithmTheme, bool _ChangeThemeAlgorithmINMessageTheme, bool _ChangeThemeAlgorithmNotStoreLastMsg, bool _IgnoreUnknownMessagesWOUnknownGeneration, bool _SplitTextIntoSentences, String[] _sMarkerInMessages, bool _LinkAdditionalThemes, bool _PlayReceiveMsg, bool _PlaySendMsg, bool _PlayMarker, bool _ShowErrorDetails, string _AllowedAdditionalThemesList, bool _MergeInMessages, bool _UseSetKoefWithLexicalCompare, int _CompareStarLevel)
        {
            CompareStarLevel = _CompareStarLevel;
            MergeInMessages = _MergeInMessages;
            UseSetKoefWithLexicalCompare = _UseSetKoefWithLexicalCompare;
            AllowedAdditionalThemesList = _AllowedAdditionalThemesList;
            ShowErrorDetails = _ShowErrorDetails;
            PlayReceiveMsg = _PlayReceiveMsg;
            PlaySendMsg = _PlaySendMsg;
            PlayMarker = _PlayMarker;

            LinkAdditionalThemes = _LinkAdditionalThemes;
            ChangeThemeAlgorithmINMessageTheme = _ChangeThemeAlgorithmINMessageTheme;
            ChangeThemeAlgorithmNotStoreLastMsg = _ChangeThemeAlgorithmNotStoreLastMsg;
            IgnoreUnknownMessagesWOUnknownGeneration = _IgnoreUnknownMessagesWOUnknownGeneration;
            SplitTextIntoSentences = _SplitTextIntoSentences;
            bLockDuplicateSend = _bLockDuplicateSend;
            bFriendsGet = _bFriendsGet;
            bFriendsAdd = _bFriendsAdd;
            bFriendsMarker = _bFriendsMarker;
            bGroupingOutMessages = _bGroupingOutMessages;
            bIgnoreMessagesFromNotContacter = _bIgnoreMessagesFromNotContacter;
            bNotGenerateUnknownMessages = _bNotGenerateUnknownMessages;
            DefaultAlgorithmTheme = _DefaultAlgorithmTheme;

            ID = _ID;
            Name = _Name;
            CompareLexicalLevel = _CompareLexicalLevel;
            CompareVectorLevel = _CompareVectorLevel;
            iMsgHarKoef = new int[_iMsgHarKoef.Length];
            for (int i = 0; i < _iMsgHarKoef.Length; i++)
                iMsgHarKoef[i] = _iMsgHarKoef[i];
            iMsgHarTypes = new int[_iMsgHarTypes.Length];
            for (int i = 0; i < _iMsgHarTypes.Length; i++)
                iMsgHarTypes[i] = _iMsgHarTypes[i];
            bMsgReplaceValue = new bool[_bMsgReplaceValue.Length];
            for (int i = 0; i < _bMsgReplaceValue.Length; i++)
                bMsgReplaceValue[i] = _bMsgReplaceValue[i];

            iMarkerAlgorithmsID = new int[_iMarkerAlgorithmsID.Length];
            for (int i = 0; i < _iMarkerAlgorithmsID.Length; i++)
                iMarkerAlgorithmsID[i] = _iMarkerAlgorithmsID[i];

            iMarkerContHarID = new int[_iMarkerContHarID.Length];
            for (int i = 0; i < _iMarkerContHarID.Length; i++)
                iMarkerContHarID[i] = _iMarkerContHarID[i];

            sMarkerContHarValues = new String[_sMarkerContHarValues.Length];
            for (int i = 0; i < _sMarkerContHarValues.Length; i++)
                sMarkerContHarValues[i] = _sMarkerContHarValues[i];

            iMarkerMsgHarID = new int[_iMarkerMsgHarID.Length];
            for (int i = 0; i < _iMarkerMsgHarID.Length; i++)
                iMarkerMsgHarID[i] = _iMarkerMsgHarID[i];

            sMarkerMsgHarValues = new String[_sMarkerMsgHarValues.Length];
            for (int i = 0; i < _sMarkerMsgHarValues.Length; i++)
                sMarkerMsgHarValues[i] = _sMarkerMsgHarValues[i];

            sMarkerInMessages = new String[_sMarkerInMessages.Length];
            for (int i = 0; i < _sMarkerInMessages.Length; i++)
                sMarkerInMessages[i] = _sMarkerInMessages[i];
        }

        public static AlgorithmsDBRecord FromRecordString(String value)
        {
            String str = value;
            int iID = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
            str = str.Substring(str.IndexOf("|") + 1);
            int iCLL = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
            str = str.Substring(str.IndexOf("|") + 1);
            int iCVL = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
            str = str.Substring(str.IndexOf("|") + 1);
            String sName = str.Substring(0, str.IndexOf("|"));
            str = str.Substring(str.IndexOf("|") + 1);

            int[] imhc = new int[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                imhc[i] = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                str = str.Substring(str.IndexOf("|") + 1);
            }
            int[] imht = new int[FormMain.iMsgHarCount];
            for (int i = 0; i < FormMain.iMsgHarCount; i++)
            {
                imht[i] = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                str = str.Substring(str.IndexOf("|") + 1);
            }

            bool[] bmrv = new bool[FormMain.iMsgHarCount];

            int[] _iMarkerAlgorithmsID = new int[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _iMarkerAlgorithmsID[i] = -1;

            int[] _iMarkerContHarID = new int[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _iMarkerContHarID[i] = 0;

            String[] _sMarkerContHarValues = new String[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _sMarkerContHarValues[i] = "";

            int[] _iMarkerMsgHarID = new int[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _iMarkerMsgHarID[i] = 0;

            String[] _sMarkerMsgHarValues = new String[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _sMarkerMsgHarValues[i] = "";

            String[] _sMarkerInMessages = new String[FormMain.MaxMarkerCount];
            for (int i = 0; i < FormMain.MaxMarkerCount; i++)
                _sMarkerInMessages[i] = "";

            bool _bLockDuplicateSend = true;
            bool _bFriendsGet = true;
            bool _bFriendsAdd = true;
            bool _bFriendsMarker = true;
            bool _bGroupingOutMessages = true;
            bool _bIgnoreMessagesFromNotContacter = false;
            bool _bNotGenerateUnknownMessages = false;
            String _DefaultAlgorithmTheme = "";
            String _AllowedAdditionalThemesList = "";
            bool _ChangeThemeAlgorithmINMessageTheme = true;
            bool _ChangeThemeAlgorithmNotStoreLastMsg = true;
            bool _IgnoreUnknownMessagesWOUnknownGeneration = false;
            bool _SplitTextIntoSentences = true;
            bool _LinkAdditionalThemes = true;
            bool _PlayReceiveMsg = true;
            bool _PlaySendMsg = true;
            bool _PlayMarker = true;
            bool _ShowErrorDetails = false;
            bool _MergeInMessages = false;
            bool _UseSetKoefWithLexicalCompare = true;
            int _CompareStarLevel = 90;

            if (str.Length > 2)
            {
                str = str.Substring(str.IndexOf("*#|") + 3);
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                {
                    bmrv[i] = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                    str = str.Substring(str.IndexOf("|") + 1);
                }

                if (str.Length > 2)
                {
                    str = str.Substring(str.IndexOf("*!|") + 3);
                    int i = 0;
                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*-|"))
                    {
                        _iMarkerAlgorithmsID[i] = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                        str = str.Substring(str.IndexOf("|") + 1);
                        i++;
                        if (str.Length == 0)
                            break;
                    }

                    if (i < FormMain.MaxMarkerCount)
                    {
                        for (i = 12; i > 7; i--)
                        {
                            _iMarkerAlgorithmsID[i] = _iMarkerAlgorithmsID[i - 3];
                        }
                        for (i = 5; i < 8; i++)
                            _iMarkerAlgorithmsID[i] = -1;
                    }

                    if (str.Length > 2)
                    {
                        str = str.Substring(str.IndexOf("*-|") + 3);
                        i = 0;
                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*+|"))
                        {
                            _iMarkerContHarID[i] = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                            str = str.Substring(str.IndexOf("|") + 1);
                            i++;
                            if (str.Length == 0)
                                break;
                        }

                        if (i < FormMain.MaxMarkerCount)
                        {
                            for (i = 12; i > 7; i--)
                            {
                                _iMarkerContHarID[i] = _iMarkerContHarID[i - 3];
                            }
                            for (i = 5; i < 8; i++)
                                _iMarkerContHarID[i] = 0;
                        }

                        if (str.Length > 2)
                        {
                            str = str.Substring(str.IndexOf("*+|") + 3);
                            i = 0;
                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*?|"))
                            {
                                _sMarkerContHarValues[i] = str.Substring(0, str.IndexOf("|"));
                                str = str.Substring(str.IndexOf("|") + 1);
                                i++;
                                if (str.Length == 0)
                                    break;
                            }

                            if (i < FormMain.MaxMarkerCount)
                            {
                                for (i = 12; i > 7; i--)
                                {
                                    _sMarkerContHarValues[i] = _sMarkerContHarValues[i - 3];
                                }
                                for (i = 5; i < 8; i++)
                                    _sMarkerContHarValues[i] = "";
                            }

                            // Next field must start with *?|
                            if (str.Length > 2)
                            {
                                str = str.Substring(str.IndexOf("*?|") + 3);
                                i = 0;
                                while (i < FormMain.MaxMarkerCount && !str.StartsWith("*$|"))
                                {
                                    _iMarkerMsgHarID[i] = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                                    str = str.Substring(str.IndexOf("|") + 1);
                                    i++;
                                    if (str.Length == 0)
                                        break;
                                }

                                if (i < FormMain.MaxMarkerCount)
                                {
                                    for (i = 12; i > 7; i--)
                                    {
                                        _iMarkerMsgHarID[i] = _iMarkerMsgHarID[i - 3];
                                    }
                                    for (i = 5; i < 8; i++)
                                        _iMarkerMsgHarID[i] = 0;
                                }

                                if (str.Length > 2)
                                {
                                    str = str.Substring(str.IndexOf("*$|") + 3);
                                    i = 0;
                                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*@|"))
                                    {
                                        _sMarkerMsgHarValues[i] = str.Substring(0, str.IndexOf("|"));
                                        str = str.Substring(str.IndexOf("|") + 1);
                                        i++;
                                        if (str.Length == 0)
                                            break;
                                    }

                                    if (i < FormMain.MaxMarkerCount)
                                    {
                                        for (i = 12; i > 7; i--)
                                        {
                                            _sMarkerMsgHarValues[i] = _sMarkerMsgHarValues[i - 3];
                                        }
                                        for (i = 5; i < 8; i++)
                                            _sMarkerMsgHarValues[i] = "";
                                    }

                                    // Next field must start with *@|
                                    if (str.Length > 2)
                                    {
                                        str = str.Substring(str.IndexOf("*@|") + 3);
                                        i = 0;
                                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^01|"))
                                        {
                                            _bLockDuplicateSend = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                            str = str.Substring(str.IndexOf("|") + 1);
                                            i++;
                                            if (str.Length == 0)
                                                break;
                                        }

                                        // Next field must start with *^01|
                                        if (str.Length > 2)
                                        {
                                            str = str.Substring(str.IndexOf("*^01|") + 5);
                                            i = 0;
                                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^02|"))
                                            {
                                                _bFriendsGet = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                str = str.Substring(str.IndexOf("|") + 1);
                                                i++;
                                                if (str.Length == 0)
                                                    break;
                                            }

                                            // Next field must start with *^02|
                                            if (str.Length > 2)
                                            {
                                                str = str.Substring(str.IndexOf("*^02|") + 5);
                                                i = 0;
                                                while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^03|"))
                                                {
                                                    _bFriendsAdd = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                    str = str.Substring(str.IndexOf("|") + 1);
                                                    i++;
                                                    if (str.Length == 0)
                                                        break;
                                                }

                                                // Next field must start with *^03|
                                                if (str.Length > 2)
                                                {
                                                    str = str.Substring(str.IndexOf("*^03|") + 5);
                                                    i = 0;
                                                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^04|"))
                                                    {
                                                        _bFriendsMarker = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                        str = str.Substring(str.IndexOf("|") + 1);
                                                        i++;
                                                        if (str.Length == 0)
                                                            break;
                                                    }

                                                    // Next field must start with *^04|
                                                    if (str.Length > 2)
                                                    {
                                                        str = str.Substring(str.IndexOf("*^04|") + 5);
                                                        i = 0;
                                                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^05|"))
                                                        {
                                                            _bGroupingOutMessages = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                            str = str.Substring(str.IndexOf("|") + 1);
                                                            i++;
                                                            if (str.Length == 0)
                                                                break;
                                                        }

                                                        // Next field must start with *^05|
                                                        if (str.Length > 2)
                                                        {
                                                            str = str.Substring(str.IndexOf("*^05|") + 5);
                                                            i = 0;
                                                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^06|"))
                                                            {
                                                                _bIgnoreMessagesFromNotContacter = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                str = str.Substring(str.IndexOf("|") + 1);
                                                                i++;
                                                                if (str.Length == 0)
                                                                    break;
                                                            }

                                                            // Next field must start with *^06|
                                                            if (str.Length > 2)
                                                            {
                                                                str = str.Substring(str.IndexOf("*^06|") + 5);
                                                                i = 0;
                                                                while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^07|"))
                                                                {
                                                                    _bNotGenerateUnknownMessages = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                    str = str.Substring(str.IndexOf("|") + 1);
                                                                    i++;
                                                                    if (str.Length == 0)
                                                                        break;
                                                                }

                                                                // Next field must start with *^07|
                                                                if (str.Length > 2)
                                                                {
                                                                    str = str.Substring(str.IndexOf("*^07|") + 5);
                                                                    i = 0;
                                                                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^08|"))
                                                                    {
                                                                        _DefaultAlgorithmTheme = str.Substring(0, str.IndexOf("|"));
                                                                        str = str.Substring(str.IndexOf("|") + 1);
                                                                        i++;
                                                                        if (str.Length == 0)
                                                                            break;
                                                                    }

                                                                    // Next field must start with *^08|
                                                                    if (str.Length > 2)
                                                                    {
                                                                        str = str.Substring(str.IndexOf("*^08|") + 5);
                                                                        i = 0;
                                                                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^09|"))
                                                                        {
                                                                            _ChangeThemeAlgorithmINMessageTheme = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                            str = str.Substring(str.IndexOf("|") + 1);
                                                                            i++;
                                                                            if (str.Length == 0)
                                                                                break;
                                                                        }

                                                                        // Next field must start with *^09|
                                                                        if (str.Length > 2)
                                                                        {
                                                                            str = str.Substring(str.IndexOf("*^09|") + 5);
                                                                            i = 0;
                                                                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^10|"))
                                                                            {
                                                                                _ChangeThemeAlgorithmNotStoreLastMsg = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                str = str.Substring(str.IndexOf("|") + 1);
                                                                                i++;
                                                                                if (str.Length == 0)
                                                                                    break;
                                                                            }

                                                                            // Next field must start with *^10|
                                                                            if (str.Length > 2)
                                                                            {
                                                                                str = str.Substring(str.IndexOf("*^10|") + 5);
                                                                                i = 0;
                                                                                while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^11|"))
                                                                                {
                                                                                    _IgnoreUnknownMessagesWOUnknownGeneration = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                    str = str.Substring(str.IndexOf("|") + 1);
                                                                                    i++;
                                                                                    if (str.Length == 0)
                                                                                        break;
                                                                                }

                                                                                // Next field must start with *^11|
                                                                                if (str.Length > 2)
                                                                                {
                                                                                    str = str.Substring(str.IndexOf("*^11|") + 5);
                                                                                    i = 0;
                                                                                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^12|"))
                                                                                    {
                                                                                        _SplitTextIntoSentences = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                        str = str.Substring(str.IndexOf("|") + 1);
                                                                                        i++;
                                                                                        if (str.Length == 0)
                                                                                            break;
                                                                                    }

                                                                                    // Next field must start with *^12|
                                                                                    if (str.Length > 2)
                                                                                    {
                                                                                        str = str.Substring(str.IndexOf("*^12|") + 5);
                                                                                        i = 0;
                                                                                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^13|"))
                                                                                        {
                                                                                            _sMarkerInMessages[i] = str.Substring(0, str.IndexOf("|"));
                                                                                            str = str.Substring(str.IndexOf("|") + 1);
                                                                                            i++;
                                                                                            if (str.Length == 0)
                                                                                                break;
                                                                                        }

                                                                                        // Next field must start with *^13|
                                                                                        if (str.Length > 2)
                                                                                        {
                                                                                            str = str.Substring(str.IndexOf("*^13|") + 5);
                                                                                            i = 0;
                                                                                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^14|"))
                                                                                            {
                                                                                                _LinkAdditionalThemes = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                str = str.Substring(str.IndexOf("|") + 1);
                                                                                                i++;
                                                                                                if (str.Length == 0)
                                                                                                    break;
                                                                                            }

                                                                                            // Next field must start with *^14|
                                                                                            if (str.Length > 2)
                                                                                            {
                                                                                                str = str.Substring(str.IndexOf("*^14|") + 5);
                                                                                                i = 0;
                                                                                                while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^15|"))
                                                                                                {
                                                                                                    _PlayReceiveMsg = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                    str = str.Substring(str.IndexOf("|") + 1);
                                                                                                    i++;
                                                                                                    if (str.Length == 0)
                                                                                                        break;
                                                                                                }

                                                                                                // Next field must start with *^15|
                                                                                                if (str.Length > 2)
                                                                                                {
                                                                                                    str = str.Substring(str.IndexOf("*^15|") + 5);
                                                                                                    i = 0;
                                                                                                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^16|"))
                                                                                                    {
                                                                                                        _PlaySendMsg = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                        str = str.Substring(str.IndexOf("|") + 1);
                                                                                                        i++;
                                                                                                        if (str.Length == 0)
                                                                                                            break;
                                                                                                    }

                                                                                                    // Next field must start with *^16|
                                                                                                    if (str.Length > 2)
                                                                                                    {
                                                                                                        str = str.Substring(str.IndexOf("*^16|") + 5);
                                                                                                        i = 0;
                                                                                                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^17|"))
                                                                                                        {
                                                                                                            _PlayMarker = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                            str = str.Substring(str.IndexOf("|") + 1);
                                                                                                            i++;
                                                                                                            if (str.Length == 0)
                                                                                                                break;
                                                                                                        }

                                                                                                        // Next field must start with *^17|
                                                                                                        if (str.Length > 2)
                                                                                                        {
                                                                                                            str = str.Substring(str.IndexOf("*^17|") + 5);
                                                                                                            i = 0;
                                                                                                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^18|"))
                                                                                                            {
                                                                                                                _ShowErrorDetails = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                                str = str.Substring(str.IndexOf("|") + 1);
                                                                                                                i++;
                                                                                                                if (str.Length == 0)
                                                                                                                    break;
                                                                                                            }

                                                                                                            // Next field must start with *^18|
                                                                                                            if (str.Length > 2)
                                                                                                            {
                                                                                                                str = str.Substring(str.IndexOf("*^18|") + 5);
                                                                                                                i = 0;
                                                                                                                while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^19|"))
                                                                                                                {
                                                                                                                    _AllowedAdditionalThemesList = str.Substring(0, str.IndexOf("|"));
                                                                                                                    str = str.Substring(str.IndexOf("|") + 1);
                                                                                                                    i++;
                                                                                                                    if (str.Length == 0)
                                                                                                                        break;
                                                                                                                }

                                                                                                                // Next field must start with *^19|
                                                                                                                
                                                                                                                if (str.Length > 2)
                                                                                                                {
                                                                                                                    str = str.Substring(str.IndexOf("*^19|") + 5);
                                                                                                                    i = 0;
                                                                                                                    while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^20|"))
                                                                                                                    {
                                                                                                                        _MergeInMessages = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                                        str = str.Substring(str.IndexOf("|") + 1);
                                                                                                                        i++;
                                                                                                                        if (str.Length == 0)
                                                                                                                            break;
                                                                                                                    }
                                                                                                                    // Next field must start with *^20|
                                                                                                                    if (str.Length > 2)
                                                                                                                    {
                                                                                                                        str = str.Substring(str.IndexOf("*^20|") + 5);
                                                                                                                        i = 0;
                                                                                                                        while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^21|"))
                                                                                                                        {
                                                                                                                            _UseSetKoefWithLexicalCompare = Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) == 1;
                                                                                                                            str = str.Substring(str.IndexOf("|") + 1);
                                                                                                                            i++;
                                                                                                                            if (str.Length == 0)
                                                                                                                                break;
                                                                                                                        }
                                                                                                                        // Next field must start with *^21|
                                                                                                                        if (str.Length > 2)
                                                                                                                        {
                                                                                                                            str = str.Substring(str.IndexOf("*^21|") + 5);
                                                                                                                            i = 0;
                                                                                                                            while (i < FormMain.MaxMarkerCount && !str.StartsWith("*^22|"))
                                                                                                                            {
                                                                                                                                _CompareStarLevel = Convert.ToInt32(str.Substring(0, str.IndexOf("|")));
                                                                                                                                str = str.Substring(str.IndexOf("|") + 1);
                                                                                                                                i++;
                                                                                                                                if (str.Length == 0)
                                                                                                                                    break;
                                                                                                                            }
                                                                                                                            // Next field must start with *^22|

                                                                                                                        }

                                                                                                                    }
                                                                                                                    // End record parsing

                                                                                                                }
                                                                                                                // End record parsing
                                                                                                            }
                                                                                                            // End record parsing
                                                                                                        }
                                                                                                        // End record parsing
                                                                                                    }

                                                                                                    // End record parsing
                                                                                                }

                                                                                                // End record parsing
                                                                                            }

                                                                                            // End record parsing
                                                                                        }
                                                                                        // End record parsing
                                                                                    }

                                                                                    // End record parsing
                                                                                }
                                                                                // End record parsing
                                                                            }
                                                                            // End record parsing
                                                                        }

                                                                        // End record parsing
                                                                    }

                                                                    // End record parsing
                                                                }
                                                                // End record parsing
                                                            }

                                                            // End record parsing
                                                        }

                                                        // End record parsing
                                                    }

                                                    // End record parsing
                                                }

                                                // End record parsing
                                            }

                                            // End record parsing
                                        }

                                        // End record parsing
                                    }

                                    // End record parsing
                                }
                            }
                            // End record parsing
                        }
                    }

                }
            }
            else
            {
                for (int i = 0; i < FormMain.iMsgHarCount; i++)
                    bmrv[i] = false;
            }

            return new AlgorithmsDBRecord(iID, sName, iCLL, iCVL, imhc, imht, bmrv, _iMarkerAlgorithmsID, _iMarkerContHarID, _sMarkerContHarValues, _iMarkerMsgHarID, _sMarkerMsgHarValues, _bLockDuplicateSend, _bFriendsGet, _bFriendsAdd, _bFriendsMarker, _bGroupingOutMessages, _bIgnoreMessagesFromNotContacter, _bNotGenerateUnknownMessages, _DefaultAlgorithmTheme, _ChangeThemeAlgorithmINMessageTheme, _ChangeThemeAlgorithmNotStoreLastMsg, _IgnoreUnknownMessagesWOUnknownGeneration, _SplitTextIntoSentences, _sMarkerInMessages, _LinkAdditionalThemes, _PlayReceiveMsg, _PlaySendMsg, _PlayMarker, _ShowErrorDetails, _AllowedAdditionalThemesList, _MergeInMessages, _UseSetKoefWithLexicalCompare, _CompareStarLevel);
        }

        public string ToRecordString()
        {
            string str = "";
            str += this.ID.ToString() + "|";
            str += this.CompareLexicalLevel.ToString() + "|";
            str += this.CompareVectorLevel.ToString() + "|";
            str += this.Name + "|";
            for (int i = 0; i < this.iMsgHarKoef.Length; i++)
                str += this.iMsgHarKoef[i].ToString() + "|";
            for (int i = 0; i < this.iMsgHarTypes.Length; i++)
                str += this.iMsgHarTypes[i].ToString() + "|";
            str += "*#|";
            for (int i = 0; i < this.iMsgHarTypes.Length; i++)
                str += (this.bMsgReplaceValue[i] ? "1" : "0") + "|";

            str += "*!|";
            for (int i = 0; i < this.iMarkerAlgorithmsID.Length; i++)
                str += (this.iMarkerAlgorithmsID[i].ToString()) + "|";

            str += "*-|";
            for (int i = 0; i < this.iMarkerContHarID.Length; i++)
                str += (this.iMarkerContHarID[i].ToString()) + "|";

            str += "*+|";
            for (int i = 0; i < this.iMarkerContHarID.Length; i++)
                str += this.sMarkerContHarValues[i] + "|";

            str += "*?|";
            for (int i = 0; i < this.iMarkerMsgHarID.Length; i++)
                str += (this.iMarkerMsgHarID[i].ToString()) + "|";

            str += "*$|";
            for (int i = 0; i < this.iMarkerMsgHarID.Length; i++)
                str += this.sMarkerMsgHarValues[i] + "|";

            str += "*@|";
            str += (this.bLockDuplicateSend ? "1" : "0") + "|";

            str += "*^01|";
            str += (this.bFriendsGet ? "1" : "0") + "|";
            str += "*^02|";
            str += (this.bFriendsAdd ? "1" : "0") + "|";
            str += "*^03|";
            str += (this.bFriendsMarker ? "1" : "0") + "|";

            str += "*^04|";
            str += (this.bGroupingOutMessages ? "1" : "0") + "|";

            str += "*^05|";
            str += (this.bIgnoreMessagesFromNotContacter ? "1" : "0") + "|";

            str += "*^06|";
            str += (this.bNotGenerateUnknownMessages ? "1" : "0") + "|";

            str += "*^07|";
            str += this.DefaultAlgorithmTheme + "|";

            str += "*^08|";
            str += (this.ChangeThemeAlgorithmINMessageTheme ? "1" : "0") + "|";

            str += "*^09|";
            str += (this.ChangeThemeAlgorithmNotStoreLastMsg ? "1" : "0") + "|";

            str += "*^10|";
            str += (this.IgnoreUnknownMessagesWOUnknownGeneration ? "1" : "0") + "|";

            str += "*^11|";
            str += (this.SplitTextIntoSentences ? "1" : "0") + "|";

            str += "*^12|";
            for (int i = 0; i < this.iMarkerMsgHarID.Length; i++)
                str += this.sMarkerInMessages[i] + "|";

            str += "*^13|";
            str += (this.LinkAdditionalThemes ? "1" : "0") + "|";

            str += "*^14|";
            str += (this.PlayReceiveMsg ? "1" : "0") + "|";
            str += "*^15|";
            str += (this.PlaySendMsg ? "1" : "0") + "|";
            str += "*^16|";
            str += (this.PlayMarker ? "1" : "0") + "|";
            str += "*^17|";
            str += (this.ShowErrorDetails ? "1" : "0") + "|";

            str += "*^18|";
            str += this.AllowedAdditionalThemesList + "|";

            str += "*^19|";
            str += (this.MergeInMessages ? "1" : "0") + "|";

            str += "*^20|";
            str += (this.UseSetKoefWithLexicalCompare ? "1" : "0") + "|";

            str += "*^21|";
            str += this.CompareStarLevel.ToString() + "|";

            return str;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
