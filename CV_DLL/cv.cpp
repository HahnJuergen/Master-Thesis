#include "pch.h"
#include "cv.h"

extern "C" 
{
	CV_PROCESSING_API int process(unsigned char * image_data, int width, int height)
	{
		cv::Mat mat = cv::Mat(height, width, CV_8UC3, image_data);
		std::vector<cv::Point> points, hull;

		Processing::instance()->perform_probabilistic_hough_transform(points, mat);
		Processing::instance()->convex_hull(hull, points);	

		if (hull.size() > 3)
		{
			for (int i = 0; i < hull.size(); i++)
				draw_line(mat, hull[i], hull[(i + 1) % hull.size()], GREEN);
		}
		
		image_data = mat.data;

		return 0;
	}
}