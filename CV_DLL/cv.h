#pragma once

#include "_processing\header\Processing.h"
#include "_utility\utility.h"

#define CV_PROCESSING_API __declspec(dllexport) 

extern "C" 
{
	CV_PROCESSING_API int process(unsigned char * image_data, int width, int height);
}