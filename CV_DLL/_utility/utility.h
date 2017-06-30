#pragma once

#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>

enum Color
{
	RED, GREEN, BLUE
};

cv::Scalar drawing_color(Color color)
{
	switch (color)
	{
	case RED: return cv::Scalar(255, 0, 0);
	case GREEN: return cv::Scalar(0, 255, 0);
	case BLUE: return cv::Scalar(0, 0, 255);
	}
}

void draw_line(cv::Mat & image, cv::Point const & p, cv::Point const & q, Color color)
{
	cv::line(image, p, q, drawing_color(color), 2);
}

void draw_circle(cv::Mat & image, cv::Point const & p, int const radius, Color color)
{
	cv::circle(image, p, radius, drawing_color(color));
}

