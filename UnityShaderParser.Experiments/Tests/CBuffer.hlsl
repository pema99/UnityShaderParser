cbuffer Foo
{
    int a = 1;
    float b = 2;
}

[Test]
void CBuffer_AccessElements_Succeeds()
{
    ASSERT(a == 1);
    ASSERT(b == 2);
}

tbuffer Bar
{
    int c = 1;
    float d = 2;
}

[Test]
void TBuffer_AccessElements_Succeeds()
{
    ASSERT(c == 1);
    ASSERT(d == 2);
}
