#include "pch.h"
#include "..\header\Processing.h"

void Processing::perform_probabilistic_hough_transform(std::vector<cv::Point> & points, cv::Mat const & image)
{
	std::vector<cv::Vec4i> lines;

	this->hough_transform(lines, image);
	this->hough_lines_points(points, lines);
}

void Processing::convex_hull(std::vector<cv::Point>& hull, std::vector<cv::Point> const & points)
{
	if (points.size() > 3)
	{
		cv::convexHull(points, hull);		
	}
}

void Processing::hough_transform(std::vector<cv::Vec4i> & lines, cv::Mat const & image)
{
	cv::Mat dst;

	cv::cvtColor(image, dst, cv::COLOR_RGB2HSV_FULL);
	cv::inRange(image, cv::Scalar(60 - 35, 110, 110), cv::Scalar(60 + 35, 255, 255), dst);

	cv::HoughLinesP(dst, lines, 1, CV_PI / 180, 35, 20, 15);
}

void Processing::hough_lines_points(std::vector<cv::Point> & points, std::vector<cv::Vec4i> const & lines)
{
	for (auto it = lines.begin(); it != lines.end(); ++it)
		for (size_t k = 1; k < 4; k += 2)
			points.push_back(cv::Point((*it)[k - 1], (*it)[k]));
}
