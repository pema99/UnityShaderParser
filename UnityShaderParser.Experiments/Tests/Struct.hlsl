static int a = 1337;

struct Foo
{
    int a;
    
    void Bar()
    {
        a += 1;
    }

    static int Baz()
    {
        return a;
    }

    void Boo(int b)
    {
        return a + b;
    }
};

[Test]
void CallInstanceMethod_OnStruct_ModifiesLocalState()
{
    Foo f;
    f.a = 12;

    f.Bar();
    ASSERT(f.a == 13);

    f.Bar();
    ASSERT(f.a == 14);
}

[Test]
void CallInstanceMethodWithParameter_OnStruct_Succeeds()
{
    Foo f;
    f.a = 12;

    ASSERT(f.Boo(5) == 17);
}


[Test]
void CallStaticMethod_OnStruct_RefersToGlobalState()
{
    ASSERT(Foo::Baz() == 1337);
}