// For cross platform wind vs linux c++ standardization support

#if defined(_MSC_VER)
    //  Microsoft 
    #define PROJECT_API __declspec(dllexport)    
#elif defined(__GNUC__)
    //  GCC
    #define PROJECT_API __attribute__((visibility("default")))    
#else
    //  do nothing and hope for the best?
    #define EXPORT
    #define IMPORT
    #pragma warning Unknown dynamic link export semantics.
#endif