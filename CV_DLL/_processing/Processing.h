#pragma once

#ifndef CV_PROCESSING_API_PROCESSING_H_
#define CV_PROCESSING_API_PROCESSING_H_

#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\calib3d\calib3d.hpp>

#include <vector>

#include "_utility\Singleton.h"
#include "_math\Math.h"
#include "_constants\Constants.h"


class Processing : public Singleton<Processing>
{

friend class Singleton<Processing>;

public:
	~Processing() {}

	void initialize_camera_related_matrices(cv::Mat & _cm, cv::Mat & _dc);
	void perform_probabilistic_hough_transform(std::vector<cv::Point> & _points, cv::Mat const & _image);
	void approximate_quadrilateral(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _points);
	void estimate_quadrilateral_pose(std::vector<double> & _rotation_vector, std::vector<double> & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients, float const _target_width, float const _target_height);
	
	int apply_data_to_out_datastructures(float * _out_corners, float * _out_rvec, float * _out_tvec, std::vector<double> const & _rotation_vector, std::vector<double> const & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral);

private:
	void _hough_transform(std::vector<cv::Vec4i> & _lines, cv::Mat const & _image);
	void _hough_lines_points(std::vector<cv::Point> & _points, std::vector<cv::Vec4i> const & _lines);
	void _convex_hull(std::vector<cv::Point> & _hull, std::vector<cv::Point> const & _points);
	void _approximate_polygon(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _hull);
	void _evaluate_detection_rejection(std::vector<cv::Point2d> & _quadrilateral);
	void _generate_object_points(std::vector<cv::Point3d> &  _object_points, float _target_width, float _target_height);
	void _estimate_rotation_vector_and_translation_vector(std::vector<double> & _rotation_vector, std::vector<double> & _translation_vector, std::vector<cv::Point3d> const & _object_points, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients);
	void _matrix_3x1_to_vector3(std::vector<double> & _vector, cv::Mat const & _matrix);
	
	template <typename _T1, typename _T2>
	void _vector_to_pointer(_T1 *_pointer, std::vector<_T2> const & _vector);

	template <typename _T1, typename _T2, int D>
	void _point_vector_to_pointer(_T1 * _pointer, std::vector<_T2> const & _vector);

protected:
	Processing() {}
};

#endif // CV_PROCESSING_API_PROCESSING_H_
