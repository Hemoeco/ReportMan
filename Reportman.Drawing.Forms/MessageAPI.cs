#region Copyright
/*
 *  Report Manager:  Database Reporting tool for .Net and Mono
 *
 *     The contents of this file are subject to the MPL License
 *     with optional use of GPL or LGPL licenses.
 *     You may not use this file except in compliance with the
 *     Licenses. You may obtain copies of the Licenses at:
 *     http://reportman.sourceforge.net/license
 *
 *     Software is distributed on an "AS IS" basis,
 *     WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations.
 *
 *  Copyright (c) 1994 - 2008 Toni Martir (toni@reportman.es)
 *  All Rights Reserved.
*/
#endregion

using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

namespace Reportman.Drawing
{

    /// 
    /// A MapiFileDesc structure contains information about a file containing a message attachment 
    /// stored as a temporary file. 
    /// 
    /// The file can contain a static OLE object, an embedded OLE object, an embedded message, 
    /// and other types of files.
    /// 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class MapiFileDesc
    {
        /// 
        /// Reserved; must be zero.
        /// 
        public uint ulReserved = 0;

        /// 
        /// Bitmask of attachment flags. Flags are MAPI_OLE and MAPI_OLE_STATIC.
        /// 
        /// If neither flag is set, the attachment is treated as a data file. 
        /// 
        public uint flFlags = 0;

        /// 
        /// An integer used to indicate where in the message text to render the attachment.
        /// 
        /// Attachments replace the character found at a certain position in the message text. 
        /// That is, attachments replace the character in the MapiMessage structure field 
        /// lpszNoteText[nPosition]. A value of – 1 (0xFFFFFFFF) means the attachment position is 
        /// not indicated; the client application will have to provide a way for the user to 
        /// access the attachment. 
        /// 
        public uint nPosition = 0xffffffff;

        /// 
        /// Pointer to the fully qualified path of the attached file.
        /// 
        /// This path should include the disk drive letter and directory name.
        /// 
        public string lpszPathName = string.Empty;

        /// 
        /// Pointer to the attachment filename seen by the recipient, which may differ from the filename in
        /// the lpszPathName member if temporary files are being used. 
        /// 
        /// If the lpszFileName member is empty or NULL, the filename from lpszPathName is used. 
        /// 
        public string lpszFileName = string.Empty;

        /// 
        /// Pointer to the attachment file type, which can be represented with a MapiFileTagExt 
        /// structure.
        /// 
        /// A value of NULL indicates an unknown file type or a file type determined by the operating system. 
        /// 
        public IntPtr lpFileType = IntPtr.Zero;
    }

    /// 
    /// MapiFileTagExt structure specifies a message attachment's type at its creation 
    /// and its current form of encoding so that it can be restored to its original type at its destination.
    /// 
    /// A MapiFileTagExt structure defines the type of an attached file for purposes such as encoding and 
    /// decoding the file, choosing the correct application to launch when opening it, or any use that 
    /// requires full information regarding the file type. 
    /// 
    /// Client applications can use information in the lpTag and lpEncoding
    /// members of this structure to determine what to do with an attachment.
    /// 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class MapiFileTagExt
    {
        /// 
        /// Reserved; must be zero.
        /// 
        public uint ulReserved = 0;

        /// 
        /// The size, in bytes, of the value defined by the lpTag member.
        /// 
        public uint cbTag = 0;

        /// 
        /// Pointer to an X.400 object identifier indicating the type of the attachment in its original form, 
        /// for example "Microsoft Excel worksheet".
        /// 
        public IntPtr lpTag = IntPtr.Zero;

        /// 
        /// The size, in bytes, of the value defined by the lpEncoding member.
        /// 
        public uint cbEncoding = 0;

        /// 
        /// Pointer to an X.400 object identifier indicating the form in which the attachment is currently 
        /// encoded, for example MacBinary, UUENCODE, or binary.
        /// 
        public IntPtr lpEncoding = IntPtr.Zero;
    }

    /// 
    /// A MapiMessage structure contains information about a message.
    /// 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class MapiMessage
    {
        /// 
        /// Reserved; must be zero.
        /// 
        public uint ulReserved = 0;

        /// 
        /// Pointer to the text string describing the message subject, 
        /// typically limited to 256 characters or less.
        /// 
        /// If this member is empty or NULL, the user has not entered subject text.
        /// 
        public string lpszSubject = string.Empty;

        /// 
        /// Pointer to a string containing the message text.
        /// 
        /// If this member is empty or NULL, there is no message text.
        /// 
        public string lpszNoteText = string.Empty;

        /// 
        /// Pointer to a string indicating a non-IPM type of message.
        /// 
        /// Client applications can select message types for their non-IPM messages. 
        /// 
        /// Clients that only support IPM messages can ignore the lpszMessageType member 
        /// when reading messages and set it to empty or NULL when sending messages.
        /// 
        public string lpszMessageType = null;

        /// 
        /// Pointer to a string indicating the date when the message was received.
        /// 
        /// The format is YYYY/MM/DD HH:MM, using a 24-hour clock.
        /// 
        public string lpszDateReceived = DateTime.Now.ToString("yyyy/MM/dd hh:mm");

        /// 
        /// Pointer to a string identifying the conversation thread to which the message belongs. 
        /// 
        /// Some messaging systems can ignore and not return this member.
        /// 
        public string lpszConversationID = string.Empty;

        /// 
        /// Bitmask of message status flags. 
        /// 
        /// The flags are MAPI_RECEIPT_REQUESTED , MAPI_SENT, 
        /// and MAPI_UNREAD.
        /// 
        public uint flFlags = 0;

        /// 
        /// Pointer to a MapiRecipDesc structure containing information about the 
        /// sender of the message.
        /// 
        public IntPtr lpOriginator = IntPtr.Zero;

        /// 
        /// The number of message recipient structures in the array pointed to by the 
        /// lpRecips member. 
        /// 
        /// A value of zero indicates no recipients are included. 
        /// 
        public uint nRecipCount = 0;

        /// 
        /// Pointer to an array of MapiRecipDesc structures, each containing 
        /// information about a message recipient.
        /// 
        public IntPtr lpRecips = IntPtr.Zero;

        /// 
        /// The number of structures describing file attachments in the array pointed to by the 
        /// lpFiles member. 
        /// 
        /// A value of zero indicates no file attachments are included.
        /// 
        public uint nFileCount = 0;

        /// 
        /// Pointer to an array of MapiFileDesc structures, each containing 
        /// information about a file attachment.
        /// 
        public IntPtr lpFiles = IntPtr.Zero;
    }

    /// 
    /// A MapiRecipDesc structure contains information about a message sender or recipient.
    /// 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class MapiRecipDesc
    {
        /// 
        /// Reserved; must be zero.
        /// 
        public uint ulReserved=0;

        /// 
        /// Contains a numeric value that indicates the type of recipient. 
        /// 
        /// Possible values are: 
        ///
        /// Value Constant Meaning
        ///
        /// 0 MAPI_ORIG Indicates the original sender of the message.
        /// 1 MAPI_TO Indicates a primary message recipient.
        /// 2 MAPI_CC Indicates a recipient of a message copy.
        /// 3 MAPI_BCC Indicates a recipient of a blind copy.
        ///
        /// 
        public uint ulRecipClass=1; // MAPI.MAPI_TO

        /// 
        /// Pointer to the display name of the message recipient or sender.
        /// 
        public string lpszName=String.Empty;

        /// 
        /// Optional pointer to the recipient or sender's address; this address is provider-specific message 
        /// delivery data. Generally, the messaging system provides such addresses for inbound messages. 
        /// 
        /// For outbound messages, the lpszAddress member can point to an address entered by the user for 
        /// a recipient not in an address book (that is, a custom recipient). 
        /// 
        /// The format of an address pointed to by the lpszAddress member is [address type][e-mail address].
        /// Examples of valid addresses are FAX:206-555-1212 and SMTP:M@X.COM. 
        /// 
        public string lpszAddress=String.Empty;

        /// 
        /// The size, in bytes, of the entry identifier pointed to by the lpEntryID member.
        /// 
        public uint ulEIDSize=0;

        /// 
        /// Pointer to an opaque entry identifier used by a messaging system service provider to identify the 
        /// message recipient. Entry identifiers have meaning only for the service provider; 
        /// client applications will not be able to decipher them. The messaging system uses this member 
        /// to return valid entry identifiers for all recipients or senders listed in the address book.
        /// 
        public IntPtr lpEntryID=IntPtr.Zero;
    }


    /// <summary>
    /// Utility class to access default Windows mail client
    /// </summary>
    public class MAPI
    {

        [DllImport("MAPI32.DLL", EntryPoint = "MAPILogon", CharSet = CharSet.Ansi)]
        private static extern UInt32 Logon(IntPtr ulUIParam, string lpszProfileName, string lpszPassword,
        UInt32 flFlags, UInt32 ulReserved, ref IntPtr lplhSession);



        [DllImport("MAPI32.DLL", EntryPoint = "MAPISendMail", CharSet = CharSet.Ansi)]
        private static extern UInt32 MAPISendMail(IntPtr lhSession, IntPtr ulUIParam,
        MapiMessage lpMessage, UInt32 flFlags, UInt32 ulReserved);


        [DllImport("MAPI32.DLL", EntryPoint = "MAPILogoff", CharSet = CharSet.Ansi)]
        private static extern uint Logoff(IntPtr lhSession, IntPtr ulUIParam, uint flFlags, uint ulReserved);



        private const int SUCCESS_SUCCESS = 0;
        private const int MAPI_USER_ABORT = 1;
        private const int MAPI_E_USER_ABORT = MAPI_USER_ABORT;
        private const int MAPI_E_FAILURE = 2;
        private const int MAPI_E_LOGIN_FAILURE = 3;
        private const int MAPI_E_LOGON_FAILURE = MAPI_E_LOGIN_FAILURE;
        private const int MAPI_E_DISK_FULL = 4;
        private const int MAPI_E_INSUFFICIENT_MEMORY = 5;
        private const int MAPI_E_BLK_TOO_SMALL = 6;
        private const int MAPI_E_TOO_MANY_SESSIONS = 8;
        private const int MAPI_E_TOO_MANY_FILES = 9;
        private const int MAPI_E_TOO_MANY_RECIPIENTS = 10;
        private const int MAPI_E_ATTACHMENT_NOT_FOUND = 11;
        private const int MAPI_E_ATTACHMENT_OPEN_FAILURE = 12;
        private const int MAPI_E_ATTACHMENT_WRITE_FAILURE = 13;
        private const int MAPI_E_UNKNOWN_RECIPIENT = 14;
        private const int MAPI_E_BAD_RECIPTYPE = 15;
        private const int MAPI_E_NO_MESSAGES = 16;
        private const int MAPI_E_INVALID_MESSAGE = 17;
        private const int MAPI_E_TEXT_TOO_LARGE = 18;
        private const int MAPI_E_INVALID_SESSION = 19;
        private const int MAPI_E_TYPE_NOT_SUPPORTED = 20;
        private const int MAPI_E_AMBIGUOUS_RECIPIENT = 21;
        private const int MAPI_E_AMBIG_RECIP = MAPI_E_AMBIGUOUS_RECIPIENT;
        private const int MAPI_E_MESSAGE_IN_USE = 22;
        private const int MAPI_E_NETWORK_FAILURE = 23;
        private const int MAPI_E_INVALID_EDITFIELDS = 24;
        private const int MAPI_E_INVALID_RECIPS = 25;
        private const int MAPI_E_NOT_SUPPORTED = 26;
        private const int MAPI_ORIG = 0;
        private const int MAPI_TO = 1;
        private const int MAPI_CC = 2;
        private const int MAPI_BCC = 3;
        //***********************
        // FLAG Declarations
        //***********************
        //* MAPILogon() flags *
        private const int MAPI_LOGON_UI = 0x1;
        private const int MAPI_NEW_SESSION = 0x2;
        private const int MAPI_FORCE_DOWNLOAD = 0x1000;
        //* MAPILogoff() flags *
        private const int MAPI_LOGOFF_SHARED = 0x1;
        private const int MAPI_LOGOFF_UI = 0x2;
        //* MAPISendMail() flags *
        private const int MAPI_DIALOG = 0x8;
        //* MAPIFindNext() flags *
        private const int MAPI_UNREAD_ONLY = 0x20;
        private const int MAPI_GUARANTEE_FIFO = 0x100;
        //* MAPIReadMail() flags *
        private const int MAPI_ENVELOPE_ONLY = 0x40;
        private const int MAPI_PEEK = 0x80;
        private const int MAPI_BODY_AS_FILE = 0x200;
        private const int MAPI_SUPPRESS_ATTACH = 0x800;
        //* MAPIDetails() flags *
        private const int MAPI_AB_NOMODIFY = 0x400;
        //* Attachment flags *
        private const int MAPI_OLE = 0x1;
        private const int MAPI_OLE_STATIC = 0x2;
        //* MapiMessage flags *
        private const int MAPI_UNREAD = 0x1;
        private const int MAPI_RECEIPT_REQUESTED = 0x2;
        private const int MAPI_SENT = 0x4;
        private static IntPtr AllocOrigin(string full_name, string address)
        {
            MapiRecipDesc recipient = new MapiRecipDesc();
            recipient.ulRecipClass = MAPI.MAPI_ORIG;
            recipient.lpszName = full_name;
            recipient.lpszAddress = address;
            recipient.ulEIDSize = 0;
            recipient.lpEntryID = IntPtr.Zero;

            Type rtype = typeof(MapiRecipDesc);
            int rsize = Marshal.SizeOf(rtype);
            IntPtr ptrr = Marshal.AllocHGlobal(rsize);

            Marshal.StructureToPtr(recipient, ptrr, false);
            return ptrr;

        }
        private static IntPtr AllocRecips(string full_name, string address,ref uint recipcount)
        {
            Strings addresslist = Strings.FromSemiColon(address);
            Strings namelist = Strings.FromSemiColon(full_name);
            while (namelist.Count < addresslist.Count)
                namelist.Add("");
            recipcount=(uint)addresslist.Count;
            MapiRecipDesc[] recipients = new MapiRecipDesc[recipcount];
            for (int i=0;i<recipcount;i++)
            {
                MapiRecipDesc recipient = new MapiRecipDesc();
                recipients[i]=recipient;
                recipient.ulRecipClass = MAPI.MAPI_TO;
                recipient.lpszName = namelist[i];
                if (recipient.lpszName == "")
                    recipient.lpszName = addresslist[i];
                recipient.lpszAddress = "SMTP:"+addresslist[i];
                recipient.ulEIDSize = 0;
                recipient.lpEntryID = IntPtr.Zero;
                recipient.ulReserved = 0;
            }

            Type rtype = typeof(MapiRecipDesc);
            int simplesize=Marshal.SizeOf(rtype);
            int rsize = simplesize*(int)recipcount;
            IntPtr ptrr = Marshal.AllocHGlobal(rsize);
            for (int i = 0; i < recipcount; i++)
            {
                Marshal.StructureToPtr(recipients[i], (IntPtr)((int)ptrr+simplesize*i), false);
            }
            return ptrr;
        }
        private static IntPtr AllocFiles(string filenames,string originalfilenames,ref uint filecount)
        {
            filecount=1;

            Strings filelist = Strings.FromSemiColon(filenames);
            Strings originalfilelist = Strings.FromSemiColon(originalfilenames);
            while (filelist.Count < originalfilelist.Count)
                filelist.Add("");
            filecount = (uint)filelist.Count;
            MapiFileDesc[] files = new MapiFileDesc[filecount];
            for (int i = 0; i < filecount; i++)
            {
                MapiFileDesc filedesc = new MapiFileDesc();
                files[i] = filedesc;
                filedesc.flFlags = 0;
                filedesc.ulReserved = 0;
                filedesc.lpszPathName = originalfilelist[i];
                filedesc.lpszFileName = filelist[i];
            }

            Type rtype = typeof(MapiFileDesc);
            int simplesize = Marshal.SizeOf(rtype);
            int rsize = simplesize * (int)filecount;
            IntPtr ptrr = Marshal.AllocHGlobal(rsize);
            for (int i = 0; i < filecount; i++)
            {
                Marshal.StructureToPtr(files[i], (IntPtr)((int)ptrr + simplesize * i), false);
            }
            return ptrr;
        }
        /// <summary>
        /// Sends a e-mail using default Windows mail client
        /// </summary>
        /// <param name="Subject"></param>
        /// Message subject
        /// <param name="Body"></param>
        /// Message body
        /// <param name="Originator"></param>
        /// Source user mail
        /// <param name="OriginatorName"></param>
        /// Source user name
        /// <param name="Recipient"></param>
        /// Destination e-mail address, multiple addresses separated by semicolon
        /// <param name="RecipientName"></param>
        /// Destination name,multiple destination names separated by semicolon
        /// <param name="file"></param>
        /// File name the destination user will see, multiple names separated by semicolon
        /// <param name="originalfile"></param>
        /// Full path to the file to send, multiple filesnames separated by semicolon
        /// <param name="showmessage"></param>
        /// Show the message to the user before sending it
        /// <returns></returns>
        public static uint SendMail(string Subject, string Body, string Originator,
            string OriginatorName, string Recipient, string RecipientName,string file,string originalfile,bool showmessage)
        {
            uint ulResult = 0;
            IntPtr hSession = IntPtr.Zero;
//            uint ulFlags = MAPI.MAPI_LOGON_UI | MAPI.MAPI_NEW_SESSION;

            ulResult = MAPI.Logon(IntPtr.Zero, null, null, 0, 0, ref hSession);
            if (Originator==null)
                Originator="";
            if (Recipient==null)
                Recipient="";
            if (file == null)
                file = "";
            if (originalfile == null)
                file = "";
            if (originalfile == "")
                file = "";
            if (ulResult == MAPI.SUCCESS_SUCCESS)
            {
                MapiMessage message = new MapiMessage();
                message.lpszSubject = Subject;
                message.lpszNoteText = Body;
                try
                {
                    if (Originator == "")
                    {
                        message.lpOriginator = IntPtr.Zero;
                    }
                    else
                    {
                        message.lpOriginator = AllocOrigin(OriginatorName, Originator);
                    }
                    if (Recipient == "")
                    {
                        message.lpRecips = IntPtr.Zero;
                        message.nRecipCount = 0;
                    }
                    else
                    {
                        uint recipcount=1;
                        message.lpRecips = AllocRecips(RecipientName, Recipient,ref recipcount);
                        message.nRecipCount = recipcount;
                    }
                    if (file == "")
                    {
                        message.lpFiles = IntPtr.Zero;
                        message.nFileCount = 0;
                    }
                    else
                    {
                        uint filecount=1;
                        message.lpFiles = AllocFiles(file, originalfile,ref filecount);
                        message.nFileCount = filecount;
                    }

                    UInt32 sendFlags = 0;
                    if (showmessage)
                        sendFlags = MAPI_DIALOG;
                    ulResult = MAPI.MAPISendMail(hSession, IntPtr.Zero, message, sendFlags, 0);

                    if (ulResult == MAPI.SUCCESS_SUCCESS)
                    {
                        ulResult = MAPI.Logoff(hSession, IntPtr.Zero, 0, 0);
                    }
                    else
                        MAPI.Logoff(hSession, IntPtr.Zero, 0, 0);
                }
                finally
                {
                    if (message.lpOriginator != IntPtr.Zero)
                        Marshal.FreeHGlobal(message.lpOriginator);
                    if (message.lpRecips != IntPtr.Zero)
                        Marshal.FreeHGlobal(message.lpRecips);
                    if (message.lpFiles != IntPtr.Zero)
                        Marshal.FreeHGlobal(message.lpFiles);
                }
            }
            return ulResult;
        }



    }

}

