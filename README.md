# UnityShaderParser
Attempting to parse Unity shaders in their entirety. Currently have parsing in place for ShaderLab DSL and HLSL, passing a wide variety of tests.

Work in progress:
- Visitor implementation for all syntax trees
- General library niceties
- Handling macros via a handrolled preprocessor

# Acknowledgements
- http://code.google.com/p/fxdis-d3d1x/ for test data
- https://github.com/James-Jones/HLSLCrossCompiler for test data
- http://developer.download.nvidia.com/shaderlibrary/webpages/shader_library.html for test data
- https://github.com/pema99/UnityShaderParser/tree/master/UnityShaderParser.Tests/TestShaders/Sdk for test data
- https://github.com/microsoft/DirectX-Graphics-Samples/ for test data
- Unity Builtin Shaders used as ShaderLab test data, available on the Unity download page
- https://github.com/tgjones/HlslTools was used as inspiration and reference for some of the HLSL parsing techniques
