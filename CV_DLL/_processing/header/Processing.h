#pragma once

#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>

#include <vector>

#include "_utility\Singleton.h"


class Processing : public Singleton<Processing>
{

friend class Singleton<Processing>;

public:
	~Processing() {}

	void perform_probabilistic_hough_transform(std::vector<cv::Point> & points, cv::Mat const & image);
	void convex_hull(std::vector<cv::Point> & hull, std::vector<cv::Point> const & points);

private:
	void hough_transform(std::vector<cv::Vec4i> & lines, cv::Mat const & image);
	void hough_lines_points(std::vector<cv::Point> & points, std::vector<cv::Vec4i> const & lines);

protected:
	Processing() {}
};

