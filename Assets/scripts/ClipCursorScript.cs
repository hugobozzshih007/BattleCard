using UnityEngine;

using System.Collections;

using System.Runtime.InteropServices;

 

public class ClipCursorScript : MonoBehaviour

{

    [DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]

    [return: MarshalAs( UnmanagedType.Bool )]

    public static extern bool ClipCursor( ref RECT rcClip );

    [DllImport( "user32.dll" )]

    [return: MarshalAs( UnmanagedType.Bool )]

    public static extern bool GetClipCursor( out RECT rcClip );

    [DllImport( "user32.dll" )]

    static extern int GetForegroundWindow( );

    [DllImport("user32.dll")]

    [return: MarshalAs( UnmanagedType.Bool )]

    static extern bool GetWindowRect( int hWnd, ref RECT lpRect );

     

    [StructLayout( LayoutKind.Sequential )]

    

    public struct RECT

    {

        public int Left;

        public int Top;

        public int Right;

        public int Bottom;

        public RECT( int left, int top, int right, int bottom )

        {

            Left = left+1;

            Top = top+1;

            Right = right-1;

            Bottom = bottom-1;

        }

    }

     

    RECT currentClippingRect;

    RECT originalClippingRect = new RECT( );

     

    void Start()

    {

        var hndl = GetForegroundWindow( );

        GetWindowRect( hndl, ref currentClippingRect );

        GetClipCursor( out originalClippingRect );

        ClipCursor( ref currentClippingRect);

    }

     

    void OnApplicationQuit()

    {

        ClipCursor( ref originalClippingRect );

    }

}
