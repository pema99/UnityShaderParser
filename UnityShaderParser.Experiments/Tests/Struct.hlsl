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

    int Boo(int b)
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

[Test]
void StructDefaultInitializer_ReadField_IsZero()
{
    Foo f = (Foo)0;
    ASSERT(f.a == 0);
}

struct Single
{
    int baz;
};

struct Dual
{
    int foo;
    Single baz;
};

[Test]
void StructScalarInitializer_ReadField_MatchesValue()
{
    Dual f = (Dual)13;
    ASSERT(f.foo == 13);
    ASSERT(f.baz.baz == 13);
}

[Test]
void StructArrayInitializer_ReadField_MatchesValues()
{
    Dual f = {2,1};
    ASSERT(f.foo == 2);
    ASSERT(f.baz.baz == 1);
}