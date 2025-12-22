// This is an include file that can be used to provide utilities when writing HLSL unit tests.

#ifndef __HLSL_TEST_INCLUDED__
#define __HLSL_TEST_INCLUDED__

    // These should do nothing when not running tests.
    #ifndef __HLSL_TEST_RUNNER__
        #define printf
        #define FORMAT(x)
        #define ASSERT(x)
        #define ASSERT_MSG(x, msg)
        #define PASS_TEST()
        #define FAIL_TEST()
        #define TEST_NAME
        #define NAMEOF(x)
    #endif

#endif