namespace UnityShaderParser.Common
{
    public struct SourceSpan
    {
        public (int Line, int Column) Start;
        public (int Line, int Column) End;

        public override string ToString() => $"({Start.Line}:{Start.Column} - {End.Line}:{End.Column})";
    }

    public struct Token<TTokenKind>
        where TTokenKind : struct
    {
        public TTokenKind Kind;
        public string? Identifier;
        public SourceSpan Span;

        public override string ToString()
        {
            if (Identifier == null)
                return Kind.ToString();
            else
                return $"{Kind}: {Identifier}";
        }
    }
}
