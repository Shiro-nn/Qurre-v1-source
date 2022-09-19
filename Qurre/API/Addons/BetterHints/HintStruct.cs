namespace Qurre.API.Addons.BetterHints
{
    public class HintStruct
    {
        public int Position { get; private set; }
        public int Voffset { get; private set; }
        public string Message { get; set; }
        public int Duration { get; private set; }
        public bool Static { get; private set; }

        public HintStruct(int pos, int vf, string msg, int dur, bool stat)
        {
            Position = pos;
            Voffset = vf;
            Message = msg;
            Duration = dur;
            Static = stat;
        }
        /*for struct

        private readonly string uid = System.Guid.NewGuid().ToString("N");
        public override bool Equals(object obj)
        {
            if (obj is not HintStruct other)
                return false;

            return this == other;
        }
        public static bool operator ==(HintStruct a, HintStruct b) => a.uid == b.uid;
        public static bool operator !=(HintStruct a, HintStruct b) => !(a == b);
        public override int GetHashCode() => System.Tuple.Create(uid).GetHashCode();
        public override string ToString()
            => $"HintStruct: Duration: \"{Duration}\"; Static: {Static}; Voffset: {Voffset}; Position: {Position}";
        */
    }
}