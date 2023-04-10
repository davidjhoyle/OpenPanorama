using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class LASPublicHeaderBlock
    {
        const int FileSignatureLen = 4;
        public string FileSignature = ""; //	File Signature (“LASF”) 	char[4]	4 bytes 	* 
        public UInt16 FileSourceID; //	File Source ID 	unsigned short	2 bytes 	* 
        public UInt16 GlobalEncoding; //	Global Encoding 	unsigned short	2 bytes 	* 
        public UInt32 ProjectIDGUIDdata1; //	Project ID - GUID data 1 	unsigned long	4 bytes 	 
        public UInt16 ProjectIDGUIDdata2; //	Project ID - GUID data 2 	unsigned short	2 byte 	 
        public UInt16 ProjectIDGUIDdata3; //	Project ID - GUID data 3 	unsigned short	2 byte 	 
        public char[] ProjectIDGUIDdata4 = new char[8]; //	Project ID - GUID data 4 	unsigned char[8]	8 bytes 	 
        public char VersionMajor; //	Version Major 	unsigned char	1 byte 	* 
        public char VersionMinor; //	Version Minor 	unsigned char	1 byte 	* 
        const int SystemIdentifierLen = 32;
        public string SystemIdentifier = "";// = new char[32]; //	System Identifier 	char[32]	32 bytes 	* 
        const int GeneratingSoftwareLen = 32;
        public string GeneratingSoftware = ""; // = new char[32]; //	Generating Software 	char[32]	32 bytes 	* 
        public UInt16 FileCreationDayofYear; //	File Creation Day of Year 	unsigned short	2 bytes 	* 
        public UInt16 FileCreationYear; //	File Creation Year 	unsigned short	2 bytes 	* 
        public UInt16 HeaderSize; //	Header Size 	unsigned short	2 bytes 	* 
        public UInt32 Offsettopointdata; //	Offset to point data 	unsigned long	4 bytes 	* 
        public UInt32 NumberofVariableLengthRecords; //	Number of Variable Length Records	unsigned long	4 bytes 	* 
        public LASPointDataRecordFormatBase.LASPointDataRecordFormatEnum PointDataRecordFormat; //	Point Data Record Format 	unsigned char	1 byte 	* 
        public UInt16 PointDataRecordLength; //	Point Data Record Length 	unsigned short	2 bytes 	* 
        public UInt32 LegacyNumberofpointrecords; //	Legacy Number of point records 	unsigned long	4 bytes 	* 
        public UInt32[] LegacyNumberofpointsbyreturn = new UInt32[5]; //	Legacy Number of points by return	unsigned long [5]	20 bytes 	* 
        public double Xscalefactor; //	X scale factor 	double	8 bytes 	* 
        public double Yscalefactor; //	Y scale factor 	double	8 bytes 	* 
        public double Zscalefactor; //	Z scale factor 	double	8 bytes 	* 
        public double Xoffset; //	X offset 	double	8 bytes 	* 
        public double Yoffset; //	Y offset 	double	8 bytes 	* 
        public double Zoffset; //	Z offset 	double	8 bytes 	* 
        public double MaxX; //	Max X 	double	8 bytes 	* 
        public double MinX; //	Min X 	double	8 bytes 	* 
        public double MaxY; //	Max Y 	double	8 bytes 	* 
        public double MinY; //	Min Y 	double	8 bytes 	* 
        public double MaxZ; //	Max Z 	double	8 bytes 	* 
        public double MinZ; //	Min Z 	double	8 bytes 	* 
        //public UInt64 StartofWaveformDataPacketRecord; //	Start of Waveform Data Packet Record	Unsigned long long	8 bytes	*
        //public UInt64 StartoffirstExtendedVariableLengthRecord; //	Start of first Extended Variable Length Record 	unsigned long long	8 bytes 	* 
        //public UInt32 NumberofExtendedVariableLengthRecords; //	Number of Extended Variable Length Records	unsigned long	4 bytes	  *
        public UInt64 Numberofpointrecords; //	Number of point records	unsigned long long	8 bytes	  *
        public UInt64[] Numberofpointsbyreturn = new UInt64[15]; //	Number of points by return	unsigned long long [15]	120 bytes	  *


        public LASPublicHeaderBlock()
        {
        }


        public void getXYZ(Int32 Xrecord, Int32 Yrecord, Int32 Zrecord, ref double Xcoordinate, ref double Ycoordinate, ref double Zcoordinate)
        {
            Xcoordinate = (Xrecord * Xscalefactor) + Xoffset;
            Ycoordinate = (Yrecord * Yscalefactor) + Yoffset;
            Zcoordinate = (Zrecord * Zscalefactor) + Zoffset;
        }

        public static LASPublicHeaderBlock Read(BinaryReader reader)
        {
            LASPublicHeaderBlock lasphb = new LASPublicHeaderBlock();
            bool done = false;
            for (int i = 0; i < LASPublicHeaderBlock.FileSignatureLen; i++)
            {
                byte bite = reader.ReadByte();
                if (bite == 0)
                {
                    done = true;
                }

                if (!done)
                {
                    lasphb.FileSignature += (char)bite;
                }
            }
            lasphb.FileSourceID = reader.ReadUInt16();
            lasphb.GlobalEncoding = reader.ReadUInt16();
            lasphb.ProjectIDGUIDdata1 = reader.ReadUInt32();
            lasphb.ProjectIDGUIDdata2 = reader.ReadUInt16();
            lasphb.ProjectIDGUIDdata3 = reader.ReadUInt16();
            for (int i = 0; i < lasphb.ProjectIDGUIDdata4.Length; i++)
            {
                lasphb.ProjectIDGUIDdata4[i] = (char)reader.ReadByte();
            }
            lasphb.VersionMajor = (char)reader.ReadByte();
            lasphb.VersionMinor = (char)reader.ReadByte();
            done = false;
            for (int i = 0; i < LASPublicHeaderBlock.SystemIdentifierLen; i++)
            {
                byte bite = reader.ReadByte();
                if (bite == 0)
                {
                    done = true;
                }

                if (!done)
                {
                    lasphb.SystemIdentifier += (char)bite;
                }
            }
            done = false;
            for (int i = 0; i < LASPublicHeaderBlock.GeneratingSoftwareLen; i++)
            {
                byte bite = reader.ReadByte();
                if (bite == 0)
                {
                    done = true;
                }

                if (!done)
                {
                    lasphb.GeneratingSoftware += (char)bite;
                }
            }
            lasphb.FileCreationDayofYear = reader.ReadUInt16();
            lasphb.FileCreationYear = reader.ReadUInt16();
            lasphb.HeaderSize = reader.ReadUInt16();
            lasphb.Offsettopointdata = reader.ReadUInt32();
            lasphb.NumberofVariableLengthRecords = reader.ReadUInt32();
            lasphb.PointDataRecordFormat = (LASPointDataRecordFormatBase.LASPointDataRecordFormatEnum)reader.ReadByte();
            lasphb.PointDataRecordLength = reader.ReadUInt16();
            lasphb.LegacyNumberofpointrecords = reader.ReadUInt32();
            for (int i = 0; i < lasphb.LegacyNumberofpointsbyreturn.Length; i++)
            {
                lasphb.LegacyNumberofpointsbyreturn[i] = reader.ReadUInt32();
            }
            lasphb.Xscalefactor = reader.ReadDouble();
            lasphb.Yscalefactor = reader.ReadDouble();
            lasphb.Zscalefactor = reader.ReadDouble();
            lasphb.Xoffset = reader.ReadDouble();
            lasphb.Yoffset = reader.ReadDouble();
            lasphb.Zoffset = reader.ReadDouble();
            lasphb.MaxX = reader.ReadDouble();
            lasphb.MinX = reader.ReadDouble();
            lasphb.MaxY = reader.ReadDouble();
            lasphb.MinY = reader.ReadDouble();
            lasphb.MaxZ = reader.ReadDouble();
            lasphb.MinZ = reader.ReadDouble();


            return lasphb;
        }
    }

    public class LASVariableLengthRecord
    {
        public UInt16 Reserved; //	unsigned short	2 bytes 
        public const int UserIDLen = 16;
        public string UserID = ""; //	char[16]	16 bytes 
        public UInt16 RecordID; //	unsigned short	2 bytes 
        public UInt16 RecordLengthAfterHeader; //	unsigned short	2 bytes 
        public const int DescriptionLen = 32;
        public string Description = ""; //	char[32]	32 bytes 
        public byte[] RecordData;
        public char[] RecordCharData;

        public LASVariableLengthRecord()
        {
        }

        public static LASVariableLengthRecord Read(BinaryReader reader)
        {
            LASVariableLengthRecord vlr = new LASVariableLengthRecord();

            vlr.Reserved = reader.ReadUInt16();
            bool done = false;
            for (int i = 0; i < LASVariableLengthRecord.UserIDLen; i++)
            {
                byte bite = reader.ReadByte();
                if (bite == 0)
                {
                    done = true;
                }

                if (!done)
                {
                    vlr.UserID += (char)bite;
                }
            }

            vlr.RecordID = reader.ReadUInt16();
            vlr.RecordLengthAfterHeader = reader.ReadUInt16();

            done = false;
            for (int i = 0; i < LASVariableLengthRecord.DescriptionLen; i++)
            {
                byte bite = reader.ReadByte();
                if (bite == 0)
                {
                    done = true;
                }

                if (!done)
                {
                    vlr.Description += (char)bite;
                }
            }

            vlr.RecordData = new byte[vlr.RecordLengthAfterHeader];
            vlr.RecordCharData = new char[vlr.RecordLengthAfterHeader];
            for (int i = 0; i < vlr.RecordLengthAfterHeader; i++)
            {
                vlr.RecordData[i] = reader.ReadByte();
                vlr.RecordCharData[i] = (char)vlr.RecordData[i];
            }

            return vlr;
        }

    }



    public enum ClassificationValue : byte
    {
        Neverclassified = 0,
        Unclassified = 1,
        Ground = 2,
        LowVegetation = 3,
        MediumVegetation = 4,
        HighVegetation = 5,
        Building = 6,
        LowPoint = 7,
        Reserved1 = 8,
        Water = 9,
        Rail = 10,
        RoadSurface = 11,
        Reserved2 = 12,
        WireGuard = 13,
        WireConductor = 14,
        TransmissionTower = 15,
        WirestructureConnector = 16,
        BridgeDeck = 17,
        HighNoise = 18,
        Reserved3 = 19,
        Userdefinable = 64
    };


    public abstract class LASPointDataRecordFormatBase
    {
        public enum LASPointDataRecordFormatEnum : byte
        {
            LASPointDataRecordFormat0ID = 0,
            LASPointDataRecordFormat1ID = 1,
            LASPointDataRecordFormat2ID = 2,
            LASPointDataRecordFormat3ID = 3,
            LASPointDataRecordFormat4ID = 4,
            LASPointDataRecordFormat5ID = 5,
            LASPointDataRecordFormat6ID = 6,
            LASPointDataRecordFormat7ID = 7,
            LASPointDataRecordFormat8ID = 8,
            LASPointDataRecordFormat9ID = 9,
            LASPointDataRecordFormat10ID = 10,
        };


        static UInt16[] PointDataRecordLengths = new UInt16[]
        {
            20,         // 0
            20+8,       // 1
            20+6,       // 2
            20+8+6,     // 3
            20+8+29,    // 4
            20+8+6+29,  // 5
            30,         // 6
            30+6,       // 7
            30+6+2,     // 8
            30+29,      // 9
            30+6+29,    // 10
        };


        public Int32 X; //	long	4 bytes 	* 
        public Int32 Y; //	long	4 bytes 	* 
        public Int32 Z; //	long	4 bytes 	* 
        public UInt16 Intensity; //	unsigned short	2 bytes 	 
        public ClassificationValue Classification; //	unsigned char	1 byte 	* 
        public byte UserData; //	unsigned char	1 byte 	 
        public UInt16 PointSourceID; //	unsigned short	2 bytes 	* 

        public LASPointDataRecordFormatBase()
        {
        }

        public virtual void Read(BinaryReader reader)
        {
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            Z = reader.ReadInt32();
            Intensity = reader.ReadUInt16();
        }

        public static void CheckSize(LASPointDataRecordFormatEnum PointDataRecordFormat, long siz)
        {
            if ((int)PointDataRecordFormat >= PointDataRecordLengths.Length || PointDataRecordLengths[(int)PointDataRecordFormat] != siz)
            {
                string err = "Create Point - Size Invalid " + PointDataRecordFormat + " Size " + siz;
                Console.WriteLine(err);
                throw (new Exception(err));
            }
        }

        public static LASPointDataRecordFormatBase Create(LASPointDataRecordFormatEnum PointDataRecordFormat, UInt16 siz)
        {
            CheckSize(PointDataRecordFormat, siz);

            switch (PointDataRecordFormat)
            {
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat0ID:
                    return new LASPointDataRecordFormat0();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat1ID:
                    return new LASPointDataRecordFormat1();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat2ID:
                    return new LASPointDataRecordFormat2();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat3ID:
                    return new LASPointDataRecordFormat3();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat4ID:
                    return new LASPointDataRecordFormat4();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat5ID:
                    return new LASPointDataRecordFormat5();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat6ID:
                    return new LASPointDataRecordFormat6();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat7ID:
                    return new LASPointDataRecordFormat7();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat8ID:
                    return new LASPointDataRecordFormat8();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat9ID:
                    return new LASPointDataRecordFormat9();
                case LASPointDataRecordFormatEnum.LASPointDataRecordFormat10ID:
                    return new LASPointDataRecordFormat10();
            }
            return null;
        }
    }




    public class LASPointDataRecordFormat0 : LASPointDataRecordFormatBase
    {
        public byte ReturnInfo; // Bitmap as detailed below:
        public byte ReturnNumber
        {
            get { return (byte)(ReturnInfo & 0x7); } //ReturnNumber;       //	3 bits (bits 0 – 2)	3 bits 	* 	
        }

        public byte NumberofReturns
        {
            get { return (byte)((ReturnInfo & 0x38) >> 3); } //NumberofReturns	; //	3 bits (bits 3 – 5)	3 bits 	* 	NumberofReturns(givenpulse)
        }

        public byte ScanDirectionFlag
        {
            get { return (byte)((ReturnInfo & 0x40) >> 6); } //ScanDirectionFlag	; //	1 bit (bit 6)	1 bit 	* 	
        }

        public byte EdgeofFlightLine
        {
            get { return (byte)((ReturnInfo & 0x80) >> 7); } ///EdgeofFlightLine	; //	1 bit (bit 7)	1 bit 	* 	
        }

        public byte ScanAngleRank; //	char	1 byte 	* 	ScanAngleRank(-90to+90)–Leftside

        public LASPointDataRecordFormat0()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;

            base.Read(reader);
            ReturnInfo = reader.ReadByte();
            Classification = (ClassificationValue)reader.ReadByte();
            ScanAngleRank = reader.ReadByte();
            UserData = reader.ReadByte();
            PointSourceID = reader.ReadUInt16();

            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat0ID, reader.BaseStream.Position - startpos);
        }
    }



    public class LASPointDataRecordFormat1 : LASPointDataRecordFormat0
    {
        public double GPSTime; //	double	8 bytes 	* 

        public LASPointDataRecordFormat1()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            GPSTime = reader.ReadDouble();
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat1ID, reader.BaseStream.Position - startpos);
        }
    }



    public class LASPointDataRecordFormat2 : LASPointDataRecordFormat0
    {
        public UInt16 Red; //	unsigned short	2 bytes 	* 			Red
        public UInt16 Green; //	unsigned short	2 bytes 	* 			Green
        public UInt16 Blue; //	unsigned short	2 bytes 	* 			Blue

        public LASPointDataRecordFormat2()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            Red = reader.ReadUInt16();
            Green = reader.ReadUInt16();
            Blue = reader.ReadUInt16();
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat2ID, reader.BaseStream.Position - startpos);
        }
    }



    public class LASPointDataRecordFormat3 : LASPointDataRecordFormat1
    {
        public UInt16 Red; //	unsigned short	2 bytes 	* 			Red
        public UInt16 Green; //	unsigned short	2 bytes 	* 			Green
        public UInt16 Blue; //	unsigned short	2 bytes 	* 			Blue

        public LASPointDataRecordFormat3()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            Red = reader.ReadUInt16();
            Green = reader.ReadUInt16();
            Blue = reader.ReadUInt16();
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat3ID, reader.BaseStream.Position - startpos);
        }
    }


    public class LASWavePacketData
    {
        public byte WavePacketDescriptorIndex; //	unsigned char	1 byte 	* 		WavePacketDescriptorIndex
        public UInt64 Byteoffsettowaveformdata; //	unsigned long long	8 bytes 	* 		Byteoffsettowaveformdata
        public UInt32 Waveformpacketsizeinbytes; //	unsigned long	4 bytes 	* 		Waveformpacketsizeinbytes
        public float ReturnPointWaveformLocation; //	float	4 bytes 	* 		ReturnPointWaveformLocation
        public float Xt; //	float	4 bytes 	* 		Xt
        public float Yt; //	float	4 bytes 	* 		Yt
        public float Zt; //	float	4 bytes 	* 		Zt

        public LASWavePacketData()
        {
        }

        public void Read(BinaryReader reader)
        {
            WavePacketDescriptorIndex = reader.ReadByte();
            Byteoffsettowaveformdata = reader.ReadUInt64();
            Waveformpacketsizeinbytes = reader.ReadUInt32();
            ReturnPointWaveformLocation = reader.ReadSingle();
            Xt = reader.ReadSingle();
            Yt = reader.ReadSingle();
            Zt = reader.ReadSingle();
        }
    }

    public class LASPointDataRecordFormat4 : LASPointDataRecordFormat1
    {
        public LASWavePacketData wpd = new LASWavePacketData();

        public LASPointDataRecordFormat4()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            wpd.Read(reader);
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat4ID, reader.BaseStream.Position - startpos);
        }
    }


    public class LASPointDataRecordFormat5 : LASPointDataRecordFormat3
    {
        public LASWavePacketData wpd = new LASWavePacketData();

        public LASPointDataRecordFormat5()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            wpd.Read(reader);
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat5ID, reader.BaseStream.Position - startpos);
        }
    }


    public class LASPointDataRecordFormat6 : LASPointDataRecordFormatBase
    {
        public byte ReturnInfo1;

        public byte ReturnNumber
        {
            get { return (byte)(ReturnInfo1 & 0x0f); } //Return Number 	4 bits(bits 0 - 3) 4 bits
        }

        public byte NumberofReturns
        {
            get { return (byte)((ReturnInfo1 & 0xf0) >> 4); } //Number of Returns(given pulse)     4 bits(bits 4 - 7) 4 bits
        }


        public byte ReturnInfo2;
        public byte ClassificationFlags
        {
            get { return (byte)(ReturnInfo1 & 0x0f); } //Classification Flags 	4 bits(bits 0 - 3) 4 bits
        }

        public byte ScannerChannel
        {
            get { return (byte)((ReturnInfo1 & 0x30) >> 4); } //Scanner Channel 	2 bits(bits 4 - 5) 2 bits
        }

        public byte ScanDirectionFlag
        {
            get { return (byte)((ReturnInfo1 & 0x40) >> 6); } //Scan Direction Flag 	1 bit(bit 6)   1 bit
        }

        public byte EdgeofFlightLine
        {
            get { return (byte)((ReturnInfo1 & 0x80) >> 7); } //Edge of Flight Line 	1 bit(bit 7)   1 bit
        }

        //Classification  unsigned char	1 byte
        //User Data unsigned char	1 byte
        public Int16 ScanAngle;         //Scan Angle short	2 bytes
        //Point Source ID     unsigned short	2 bytes
        public double GPSTime;          //GPS Time double	8 bytes


        public LASPointDataRecordFormat6()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            ReturnInfo1 = reader.ReadByte();
            ReturnInfo2 = reader.ReadByte();
            Classification = (ClassificationValue)reader.ReadByte();
            UserData = reader.ReadByte();
            ScanAngle = reader.ReadInt16();
            PointSourceID = reader.ReadUInt16();
            GPSTime = reader.ReadDouble();
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat6ID, reader.BaseStream.Position - startpos);
        }
    }



    public class LASPointDataRecordFormat7 : LASPointDataRecordFormat6
    {
        public UInt16 Red; //	unsigned short	2 bytes 	* 			Red
        public UInt16 Green; //	unsigned short	2 bytes 	* 			Green
        public UInt16 Blue; //	unsigned short	2 bytes 	* 			Blue


        public LASPointDataRecordFormat7()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            Red = reader.ReadUInt16();
            Green = reader.ReadUInt16();
            Blue = reader.ReadUInt16();
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat7ID, reader.BaseStream.Position - startpos);
        }
    }


    public class LASPointDataRecordFormat8 : LASPointDataRecordFormat7
    {
        public UInt16 NIR; //	NIR unsigned short	2 bytes


        public LASPointDataRecordFormat8()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            NIR = reader.ReadUInt16();
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat8ID, reader.BaseStream.Position - startpos);
        }
    }



    public class LASPointDataRecordFormat9 : LASPointDataRecordFormat6
    {
        public LASWavePacketData wpd = new LASWavePacketData();

        public LASPointDataRecordFormat9()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            wpd.Read(reader);
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat9ID, reader.BaseStream.Position - startpos);
        }
    }

    public class LASPointDataRecordFormat10 : LASPointDataRecordFormat7
    {
        public LASWavePacketData wpd = new LASWavePacketData();

        public LASPointDataRecordFormat10()
        {
        }

        public override void Read(BinaryReader reader)
        {
            long startpos = reader.BaseStream.Position;
            base.Read(reader);
            wpd.Read(reader);
            CheckSize(LASPointDataRecordFormatEnum.LASPointDataRecordFormat10ID, reader.BaseStream.Position - startpos);
        }
    }


    public class LASVariableLengthRecordHeader
    {
        //Item Format Size Required
        public UInt16 Reserved; //	unsigned short	2 bytes 	 
        public const int UserIDLen = 16; //			
        public string UserID; //	char[16]	16 bytes 	* 
        public UInt16 RecordID; //	unsigned short	2 bytes 	* 
        public UInt16 RecordLengthAfterHeader; //	unsigned short	2 bytes 	* 
        public const int DescriptionLen = 16; //			
        public string Description; //	char[32]	32 bytes 	 
    }



    public class LASExtendedVariableLengthRecord
    {
    }



    public class LASExtendedVariableLengthRecordHeader
    {
        public UInt16 Reserved;  //	unsigned short	2 bytes 	 
        public const int UserIDLen = 16;
        public string UserID;   //	char[16]	16 bytes 	* 
        public UInt16 RecordID; //	unsigned short	2 bytes 	* 
        public UInt64 RecordLengthAfterHeader; //	unsigned long long	8 bytes 	* 
        public const int DescriptionLen = 16;
        public string Description; //	char[32]	32 bytes 	 
    }


    public class LASClass
    {
        public LASPublicHeaderBlock phb;
        public LASVariableLengthRecord[] vlr;
        public LASExtendedVariableLengthRecord[] evlr;
        public LASPointDataRecordFormatBase[] pdrf;

        BinaryReader reader;

        public LASClass()
        {
        }

        public static LASClass Read(BinaryReader _reader)
        {
            LASClass las = new LASClass();
            las.reader = _reader;
            las.phb = LASPublicHeaderBlock.Read(las.reader);
            las.vlr = new LASVariableLengthRecord[1];
            las.vlr[0] = LASVariableLengthRecord.Read(las.reader);

            UInt64 numPtRecords = las.phb.LegacyNumberofpointrecords;
            if (las.phb.Numberofpointrecords > numPtRecords)
            {
                numPtRecords = las.phb.Numberofpointrecords;
            }

            las.pdrf = new LASPointDataRecordFormatBase[numPtRecords];

            for (UInt64 i = 0; i < numPtRecords; i++)
            {
                las.pdrf[i] = LASPointDataRecordFormatBase.Create(las.phb.PointDataRecordFormat, las.phb.PointDataRecordLength);
                las.pdrf[i].Read(las.reader);
            }

            return las;
        }



        public void CreateASCFile(double lat, double lon, double distance, string outputfile, double resolution, countryEnum eCountry)
        {
            // First get the starting position in NE units...
            LatLon_OsGridRef ll = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne = ll.toGrid(eCountry);
            if (ne == null)
            {
                return;
            }

            double x = 0;
            double y = 0;
            double z = 0;

            double min_x = (int)phb.MinX;
            double max_x = (int)phb.MaxX;
            double min_y = (int)phb.MinY;
            double max_y = (int)phb.MaxY;

            if (distance > 0)
            {
                min_x = (int)(ne.east - (distance));
                max_x = min_x + 2 * distance;
                min_y = (int)(ne.north - (distance));
                max_y = min_y + 2 * distance;
            }

            LidarBlock asc = new LidarBlock(outputfile, (int)(max_x - min_x + 1), (int)(max_y - min_y + 1));
            asc.xllcorner = min_x;
            asc.yllcorner = min_y;
            asc.cellsize = resolution;

            if (pdrf != null)
            {
                for (int i = 0; i < pdrf.Length; i++)
                {
                    LASPointDataRecordFormatBase pt = pdrf[i];
                    phb.getXYZ(pt.X, pt.Y, pt.Z, ref x, ref y, ref z);

                    if (x >= min_x && x <= max_x && y >= min_y && y < max_y)
                    {
                        int indexX = (int)((x - min_x) / resolution);
                        int indexY = (int)((y - min_y) / resolution);

                        if (indexX >= 0 && indexX < asc.ncols && indexY >= 0 && indexY < asc.nrows)
                        {
                            if (asc.values[indexY][indexX] == asc.NODATA_value)
                            {
                                asc.values[indexY][indexX] = (float)z;
                            }
                            else if (z < asc.values[indexY][indexX])
                            {
                                asc.values[indexY][indexX] = (float)z;
                            }
                        }
                    }
                }
            }

            StreamWriter stream = new StreamWriter(outputfile);
            asc.WriteASC(outputfile, stream);
            stream.Close();
        }
    }
}
