//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PanaGraph
//{
//    public enum KnownColor
//    {
//        //
//        // Summary:
//        //     The system-defined color of the active window's border.
//        ActiveBorder = 1,
//        //
//        // Summary:
//        //     The system-defined color of the background of the active window's title bar.
//        ActiveCaption,
//        //
//        // Summary:
//        //     The system-defined color of the text in the active window's title bar.
//        ActiveCaptionText,
//        //
//        // Summary:
//        //     The system-defined color of the application workspace. The application workspace
//        //     is the area in a multiple-document view that is not being occupied by documents.
//        AppWorkspace,
//        //
//        // Summary:
//        //     The system-defined face color of a 3-D element.
//        Control,
//        //
//        // Summary:
//        //     The system-defined shadow color of a 3-D element. The shadow color is applied
//        //     to parts of a 3-D element that face away from the light source.
//        ControlDark,
//        //
//        // Summary:
//        //     The system-defined color that is the dark shadow color of a 3-D element. The
//        //     dark shadow color is applied to the parts of a 3-D element that are the darkest
//        //     color.
//        ControlDarkDark,
//        //
//        // Summary:
//        //     The system-defined color that is the light color of a 3-D element. The light
//        //     color is applied to parts of a 3-D element that face the light source.
//        ControlLight,
//        //
//        // Summary:
//        //     The system-defined highlight color of a 3-D element. The highlight color is applied
//        //     to the parts of a 3-D element that are the lightest color.
//        ControlLightLight,
//        //
//        // Summary:
//        //     The system-defined color of text in a 3-D element.
//        ControlText,
//        //
//        // Summary:
//        //     The system-defined color of the desktop.
//        Desktop,
//        //
//        // Summary:
//        //     The system-defined color of dimmed text. Items in a list that are disabled are
//        //     displayed in dimmed text.
//        GrayText,
//        //
//        // Summary:
//        //     The system-defined color of the background of selected items. This includes selected
//        //     menu items as well as selected text.
//        Highlight,
//        //
//        // Summary:
//        //     The system-defined color of the text of selected items.
//        HighlightText,
//        //
//        // Summary:
//        //     The system-defined color used to designate a hot-tracked item. Single-clicking
//        //     a hot-tracked item executes the item.
//        HotTrack,
//        //
//        // Summary:
//        //     The system-defined color of an inactive window's border.
//        InactiveBorder,
//        //
//        // Summary:
//        //     The system-defined color of the background of an inactive window's title bar.
//        InactiveCaption,
//        //
//        // Summary:
//        //     The system-defined color of the text in an inactive window's title bar.
//        InactiveCaptionText,
//        //
//        // Summary:
//        //     The system-defined color of the background of a ToolTip.
//        Info,
//        //
//        // Summary:
//        //     The system-defined color of the text of a ToolTip.
//        InfoText,
//        //
//        // Summary:
//        //     The system-defined color of a menu's background.
//        Menu,
//        //
//        // Summary:
//        //     The system-defined color of a menu's text.
//        MenuText,
//        //
//        // Summary:
//        //     The system-defined color of the background of a scroll bar.
//        ScrollBar,
//        //
//        // Summary:
//        //     The system-defined color of the background in the client area of a window.
//        Window,
//        //
//        // Summary:
//        //     The system-defined color of a window frame.
//        WindowFrame,
//        //
//        // Summary:
//        //     The system-defined color of the text in the client area of a window.
//        WindowText,
//        //
//        // Summary:
//        //     A system-defined color.
//        Transparent,
//        //
//        // Summary:
//        //     A system-defined color.
//        AliceBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        AntiqueWhite,
//        //
//        // Summary:
//        //     A system-defined color.
//        Aqua,
//        //
//        // Summary:
//        //     A system-defined color.
//        Aquamarine,
//        //
//        // Summary:
//        //     A system-defined color.
//        Azure,
//        //
//        // Summary:
//        //     A system-defined color.
//        Beige,
//        //
//        // Summary:
//        //     A system-defined color.
//        Bisque,
//        //
//        // Summary:
//        //     A system-defined color.
//        Black,
//        //
//        // Summary:
//        //     A system-defined color.
//        BlanchedAlmond,
//        //
//        // Summary:
//        //     A system-defined color.
//        Blue,
//        //
//        // Summary:
//        //     A system-defined color.
//        BlueViolet,
//        //
//        // Summary:
//        //     A system-defined color.
//        Brown,
//        //
//        // Summary:
//        //     A system-defined color.
//        BurlyWood,
//        //
//        // Summary:
//        //     A system-defined color.
//        CadetBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        Chartreuse,
//        //
//        // Summary:
//        //     A system-defined color.
//        Chocolate,
//        //
//        // Summary:
//        //     A system-defined color.
//        Coral,
//        //
//        // Summary:
//        //     A system-defined color.
//        CornflowerBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        Cornsilk,
//        //
//        // Summary:
//        //     A system-defined color.
//        Crimson,
//        //
//        // Summary:
//        //     A system-defined color.
//        Cyan,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkCyan,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkGoldenrod,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkGray,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkKhaki,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkMagenta,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkOliveGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkOrange,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkOrchid,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkRed,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkSalmon,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkSeaGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkSlateBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkSlateGray,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkTurquoise,
//        //
//        // Summary:
//        //     A system-defined color.
//        DarkViolet,
//        //
//        // Summary:
//        //     A system-defined color.
//        DeepPink,
//        //
//        // Summary:
//        //     A system-defined color.
//        DeepSkyBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        DimGray,
//        //
//        // Summary:
//        //     A system-defined color.
//        DodgerBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        Firebrick,
//        //
//        // Summary:
//        //     A system-defined color.
//        FloralWhite,
//        //
//        // Summary:
//        //     A system-defined color.
//        ForestGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        Fuchsia,
//        //
//        // Summary:
//        //     A system-defined color.
//        Gainsboro,
//        //
//        // Summary:
//        //     A system-defined color.
//        GhostWhite,
//        //
//        // Summary:
//        //     A system-defined color.
//        Gold,
//        //
//        // Summary:
//        //     A system-defined color.
//        Goldenrod,
//        //
//        // Summary:
//        //     A system-defined color.
//        Gray,
//        //
//        // Summary:
//        //     A system-defined color.
//        Green,
//        //
//        // Summary:
//        //     A system-defined color.
//        GreenYellow,
//        //
//        // Summary:
//        //     A system-defined color.
//        Honeydew,
//        //
//        // Summary:
//        //     A system-defined color.
//        HotPink,
//        //
//        // Summary:
//        //     A system-defined color.
//        IndianRed,
//        //
//        // Summary:
//        //     A system-defined color.
//        Indigo,
//        //
//        // Summary:
//        //     A system-defined color.
//        Ivory,
//        //
//        // Summary:
//        //     A system-defined color.
//        Khaki,
//        //
//        // Summary:
//        //     A system-defined color.
//        Lavender,
//        //
//        // Summary:
//        //     A system-defined color.
//        LavenderBlush,
//        //
//        // Summary:
//        //     A system-defined color.
//        LawnGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        LemonChiffon,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightCoral,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightCyan,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightGoldenrodYellow,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightGray,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightPink,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightSalmon,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightSeaGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightSkyBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightSlateGray,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightSteelBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        LightYellow,
//        //
//        // Summary:
//        //     A system-defined color.
//        Lime,
//        //
//        // Summary:
//        //     A system-defined color.
//        LimeGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        Linen,
//        //
//        // Summary:
//        //     A system-defined color.
//        Magenta,
//        //
//        // Summary:
//        //     A system-defined color.
//        Maroon,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumAquamarine,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumOrchid,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumPurple,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumSeaGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumSlateBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumSpringGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumTurquoise,
//        //
//        // Summary:
//        //     A system-defined color.
//        MediumVioletRed,
//        //
//        // Summary:
//        //     A system-defined color.
//        MidnightBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        MintCream,
//        //
//        // Summary:
//        //     A system-defined color.
//        MistyRose,
//        //
//        // Summary:
//        //     A system-defined color.
//        Moccasin,
//        //
//        // Summary:
//        //     A system-defined color.
//        NavajoWhite,
//        //
//        // Summary:
//        //     A system-defined color.
//        Navy,
//        //
//        // Summary:
//        //     A system-defined color.
//        OldLace,
//        //
//        // Summary:
//        //     A system-defined color.
//        Olive,
//        //
//        // Summary:
//        //     A system-defined color.
//        OliveDrab,
//        //
//        // Summary:
//        //     A system-defined color.
//        Orange,
//        //
//        // Summary:
//        //     A system-defined color.
//        OrangeRed,
//        //
//        // Summary:
//        //     A system-defined color.
//        Orchid,
//        //
//        // Summary:
//        //     A system-defined color.
//        PaleGoldenrod,
//        //
//        // Summary:
//        //     A system-defined color.
//        PaleGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        PaleTurquoise,
//        //
//        // Summary:
//        //     A system-defined color.
//        PaleVioletRed,
//        //
//        // Summary:
//        //     A system-defined color.
//        PapayaWhip,
//        //
//        // Summary:
//        //     A system-defined color.
//        PeachPuff,
//        //
//        // Summary:
//        //     A system-defined color.
//        Peru,
//        //
//        // Summary:
//        //     A system-defined color.
//        Pink,
//        //
//        // Summary:
//        //     A system-defined color.
//        Plum,
//        //
//        // Summary:
//        //     A system-defined color.
//        PowderBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        Purple,
//        //
//        // Summary:
//        //     A system-defined color.
//        Red,
//        //
//        // Summary:
//        //     A system-defined color.
//        RosyBrown,
//        //
//        // Summary:
//        //     A system-defined color.
//        RoyalBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        SaddleBrown,
//        //
//        // Summary:
//        //     A system-defined color.
//        Salmon,
//        //
//        // Summary:
//        //     A system-defined color.
//        SandyBrown,
//        //
//        // Summary:
//        //     A system-defined color.
//        SeaGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        SeaShell,
//        //
//        // Summary:
//        //     A system-defined color.
//        Sienna,
//        //
//        // Summary:
//        //     A system-defined color.
//        Silver,
//        //
//        // Summary:
//        //     A system-defined color.
//        SkyBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        SlateBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        SlateGray,
//        //
//        // Summary:
//        //     A system-defined color.
//        Snow,
//        //
//        // Summary:
//        //     A system-defined color.
//        SpringGreen,
//        //
//        // Summary:
//        //     A system-defined color.
//        SteelBlue,
//        //
//        // Summary:
//        //     A system-defined color.
//        Tan,
//        //
//        // Summary:
//        //     A system-defined color.
//        Teal,
//        //
//        // Summary:
//        //     A system-defined color.
//        Thistle,
//        //
//        // Summary:
//        //     A system-defined color.
//        Tomato,
//        //
//        // Summary:
//        //     A system-defined color.
//        Turquoise,
//        //
//        // Summary:
//        //     A system-defined color.
//        Violet,
//        //
//        // Summary:
//        //     A system-defined color.
//        Wheat,
//        //
//        // Summary:
//        //     A system-defined color.
//        White,
//        //
//        // Summary:
//        //     A system-defined color.
//        WhiteSmoke,
//        //
//        // Summary:
//        //     A system-defined color.
//        Yellow,
//        //
//        // Summary:
//        //     A system-defined color.
//        YellowGreen,
//        //
//        // Summary:
//        //     The system-defined face color of a 3-D element.
//        ButtonFace,
//        //
//        // Summary:
//        //     The system-defined color that is the highlight color of a 3-D element. This color
//        //     is applied to parts of a 3-D element that face the light source.
//        ButtonHighlight,
//        //
//        // Summary:
//        //     The system-defined color that is the shadow color of a 3-D element. This color
//        //     is applied to parts of a 3-D element that face away from the light source.
//        ButtonShadow,
//        //
//        // Summary:
//        //     The system-defined color of the lightest color in the color gradient of an active
//        //     window's title bar.
//        GradientActiveCaption,
//        //
//        // Summary:
//        //     The system-defined color of the lightest color in the color gradient of an inactive
//        //     window's title bar.
//        GradientInactiveCaption,
//        //
//        // Summary:
//        //     The system-defined color of the background of a menu bar.
//        MenuBar,
//        //
//        // Summary:
//        //     The system-defined color used to highlight menu items when the menu appears as
//        //     a flat menu.
//        MenuHighlight
//    }
//}
