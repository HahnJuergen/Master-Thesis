#pragma once

#ifndef CV_PROCESSING_API_PROCESSING_H_
#define CV_PROCESSING_API_PROCESSING_H_

#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\calib3d\calib3d.hpp>

#include <vector>
#include <algorithm>

#include "_utility\Singleton.h"
#include "_math\Math.h"
#include "_constants\Constants.h"


class Processing : public Singleton<Processing>
{

friend class Singleton<Processing>;

public:
	~Processing() {}

	void initialize_camera_related_matrices(cv::Mat & _cm, cv::Mat & _dc);
	void perform_probabilistic_hough_transform(std::vector<cv::Point> & _points, cv::Mat & _image);
	void approximate_polygon(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _points, uint32_t const _accuracy);
	void estimate_quadrilateral_pose(Quaternion<double> & _rotation, std::vector<double> & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients, float const _target_width, float const _target_height);
	
	uint32_t apply_data_to_out_datastructures(float * _out_corners, float * _out_rvec, float * _out_tvec, Quaternion<double> const & _rotation, std::vector<double> const & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral);

private:
	template <typename _T1, typename _T2>
	void _classify_extreme_point_pair_vertical_edge(_T1 & _t1, _T1 & _t2, _T1 const & _s1, _T1 const & _s2, _T2 const & _c1, _T2 const & _c2);

	template <typename _T1, typename _T2>
	void _vector_to_pointer(_T1 *_pointer, std::vector<_T2> const & _vector);

	template <typename _T1, typename _T2, uint32_t D>
	void _point_vector_to_pointer(_T1 * _pointer, std::vector<_T2> const & _vector);

	void _hough_transform(std::vector<cv::Vec4i> & _lines, cv::Mat const & _image);
	void _hough_lines_points(std::vector<cv::Point> & _points, std::vector<cv::Vec4i> const & _lines);
	void _convex_hull(std::vector<cv::Point> & _hull, std::vector<cv::Point> const & _points);
	void _approximate_quadrilateral(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> & _hull, uint32_t const _accuracy);
	void _compute_edges(std::vector<cv::Vec4d> & _edges, std::vector<cv::Point> const & _hull, std::vector<cv::Point2d> const & _quadrilateral);
	void _retrieve_points_of_edges(std::vector<std::vector<cv::Point>> & _edges, std::vector<cv::Point> const & _hull, std::vector<cv::Point2d> const & _quadrilateral);
	void _fit_edge_line(std::vector<cv::Vec4d> & _fit_edges, std::vector<std::vector<cv::Point>> const & _edges);
	void _accumulate_convex_hull(std::vector<cv::Point> & _hull, std::vector<cv::Point> const & _intersections);
	void _approximate_polygon_from_convex_hull(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _hull);
	void _unify_quadrilateral_corner_descriptors(std::vector<cv::Point2d> & _quadrilateral);
	void _evaluate_detection_rejection(std::vector<cv::Point2d> & _quadrilateral);
	void _generate_object_points(cv::Mat &  _object_points, float const _target_width, float const _target_height);
	void _estimate_rotation_vector_and_translation_vector(std::vector<double> & _rotation_vector, std::vector<double> & _translation_vector, cv::Mat const & _object_points, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients);
	void _rodrigues_rotation_vector_to_quaternion(Quaternion<double> & _rotation, std::vector<double> const & _rotation_vector);	
	
	void _matrix_to_vector(std::vector<double> & _vector, cv::Mat const & _matrix);
	bool _intersect_fitted_edges(std::vector<cv::Point> & _intersections, std::vector<cv::Vec4d> const & _fit_edges);

protected:
	Processing() {}
};

#endif // CV_PROCESSING_API_PROCESSING_H_
