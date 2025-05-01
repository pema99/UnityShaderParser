// https://docs.unity3d.com/2022.3/Documentation/Manual/SL-PackageRequirements.html
Shader "Package Requirements"
{
    SubShader
    {
        PackageRequirements
        {
            "com.some.package.x": "[2.3.4,3.4.5]"
            "com.some.package.z" : "[1.1,3.2]"
            "unity" : "2021.2"
        }
        Pass
        {
            PackageRequirements
            {
                "com.some.package.y": "[1.2.2,2.5]"
                "com.some.package.z" : "[2.0,3.1]"
                "com.some.package.w" : "unity=[2021.2.1,2021.2.5]"
            }
        }
    }
}