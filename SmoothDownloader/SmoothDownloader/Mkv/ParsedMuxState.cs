using System.Collections.Generic;
using System.Text;

using SmoothDownloader.Smooth;

namespace SmoothDownloader.Mkv
{
    public class ParsedMuxState
    {
        public string Status = "unparsed";
        public bool HasZ;
        public ulong Vz = 0;
        public bool HasM;
        public ulong Vm = 0;
        public bool HasS;
        public ulong Vs = 0;
        public bool HasA;
        public ulong Va = 0;
        public bool IsXGood = false;
        public bool HasX;
        public ulong Vx = 0;
        public bool HasV;
        public ulong Vv = 0;
        public bool HasH;
        public byte[] Vh = null;
        public IList<CuePoint> CuePoints = null;
        public bool IsComplete = false;
        public bool IsContinuable = false;
        public ulong LastOutOfs = 0;
        public bool HasC;
        public ulong LastC = 0;

        public ulong[] TrackLastStartTimes = null;
        public IList<ulong>[] TrackChunkStartTimes = null;
        public int EndOffset = 0;
        public ParsedMuxState()
        {
            HasZ = HasM = HasS = HasX = HasV = HasH = HasC = HasA = false;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder("ParsedMuxState(status=" + Utils.EscapeString(Status));

            if (IsComplete)
            {
                builder.Append(", Complete");
            }
            else if (IsContinuable)
            {
                builder.Append(", Continuable");
            }
            else
            {
                builder.Append(", Unusable");
            }

            if (IsXGood)
            {
                builder.Append(", XGood");
            }
            else if (HasX)
            {
                builder.Append(", X=").Append(Vx);
            }

            if (HasS)
            {
                builder.Append(", S=").Append(Vs);
            }

            if (HasH)
            {
                builder.Append(", H.size=").Append(Vh.Length);
            }

            if (HasA)
            {
                builder.Append(", A=").Append(Va);
            }

            if (HasV)
            {
                builder.Append(", V=" + Vv);
            }

            if (HasC)
            {
                builder.Append(", lastC=" + LastC);
            }

            if (HasM)
            {
                builder.Append(", M=" + Vm);
            }

            if (HasZ)
            {
                builder.Append(", Z=" + Vz);
            }

            if (CuePoints != null)
            {
                builder.Append(", cuePoints.size=").Append(CuePoints.Count);
            }

            if (LastOutOfs > 0)
            {
                builder.Append(", lastOutOfs=" + LastOutOfs);
            }

            if (TrackLastStartTimes != null)
            {
                for (var position = 0; position < TrackLastStartTimes.Length; ++position)
                {
                    builder.Append(", lastStartTime[").Append(position).Append("]=").Append(TrackLastStartTimes[position]);
                }
            }

            if (TrackChunkStartTimes != null)
            {
                for (var position = 0; position < TrackChunkStartTimes.Length; ++position)
                {
                    builder.Append(", chunkStartTime[").Append(position).Append("].Size=").Append(TrackChunkStartTimes[position].Count);
                }
            }

            return builder.Append(")").ToString();
        }
    }
}