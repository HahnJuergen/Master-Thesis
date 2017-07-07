#include "pch.h"
#include "Processing.h"

void Processing::initialize_camera_related_matrices(cv::Mat & _cm, cv::Mat & _dc)
{
	_cm = cv::Mat();
	
	_cm.create(3, 3, CV_64F);
	_cm.at<double>(0, 0) = 1006.224;
	_cm.at<double>(0, 1) = 0.0;
	_cm.at<double>(0, 2) = 448.0;
	_cm.at<double>(1, 0) = 0.0;
	_cm.at<double>(1, 1) = 1006.224;
	_cm.at<double>(1, 2) = 252.0;
	_cm.at<double>(2, 0) = 0.0;
	_cm.at<double>(2, 1) = 0.0;
	_cm.at<double>(2, 2) = 1.0;

	_dc = cv::Mat();

	_dc.create(5, 1, CV_64F);
	_dc.at<double>(0, 0) = -0.005678121;
	_dc.at<double>(0, 1) = -1.156654;
	_dc.at<double>(0, 2) = -0.001384972;
	_dc.at<double>(0, 3) = -0.003928866;
	_dc.at<double>(0, 4) = 9.449977;
}

void Processing::perform_probabilistic_hough_transform(std::vector<cv::Point> & _points, cv::Mat const & _image)
{
	std::vector<cv::Vec4i> lines;

	this->_hough_transform(lines, _image);
	this->_hough_lines_points(_points, lines);
}

void Processing::approximate_quadrilateral(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _points)
{
	std::vector<cv::Point> hull;

	this->_convex_hull(hull, _points);
	this->_approximate_polygon(_quadrilateral, hull);
	this->_evaluate_detection_rejection(_quadrilateral);
}

void Processing::estimate_quadrilateral_pose(std::vector<double> & _rotation_vector, std::vector<double> & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients, float const _target_width, float const _target_height)
{
	if (_quadrilateral.size() == cnst::processing::SUFFICIENT_NUMBER_CORNERS)
	{
		std::vector<cv::Point3d> object_points;

		this->_generate_object_points(object_points, _target_width, _target_height);
		this->_estimate_rotation_vector_and_translation_vector(_rotation_vector, _translation_vector, object_points, _quadrilateral, _camera_matrix, _distortion_coefficients);
	}
}

int Processing::apply_data_to_out_datastructures(float * _out_corners, float * _out_rvec, float * _out_tvec, std::vector<double> const & _rotation_vector, std::vector<double> const & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral)
{
	if (_quadrilateral.size() == 4)
	{
		this->_vector_to_pointer(_out_rvec, _rotation_vector);
		this->_vector_to_pointer(_out_tvec, _translation_vector);
		this->_point_vector_to_pointer<float, cv::Point2d, 2>(_out_corners, _quadrilateral);
		
		return 0;
	}
	else
	{
		return 1;
	}
}

void Processing::_convex_hull(std::vector<cv::Point> & _hull, std::vector<cv::Point> const & _points)
{
	if (_points.size() > (cnst::processing::SUFFICIENT_NUMBER_CORNERS - 1))
		cv::convexHull(_points, _hull);
}

void Processing::_approximate_polygon(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _hull)
{
	if (_hull.size() > (cnst::processing::SUFFICIENT_NUMBER_CORNERS - 1))
	{
		double epsilon = cnst::processing::ARC_LENGTH_MULTIPLICATOR * cv::arcLength(_hull, true);
		cv::approxPolyDP(_hull, _quadrilateral, epsilon, true);
	}
}

void Processing::_evaluate_detection_rejection(std::vector<cv::Point2d> & _quadrilateral)
{
	if (_quadrilateral.size() == cnst::processing::SUFFICIENT_NUMBER_CORNERS)
	{
		float a = Math::instance()->angle_degrees<float>(_quadrilateral[3] - _quadrilateral[0], _quadrilateral[1] - _quadrilateral[0]);
		float b = Math::instance()->angle_degrees<float>(_quadrilateral[0] - _quadrilateral[1], _quadrilateral[2] - _quadrilateral[1]);
		float c = Math::instance()->angle_degrees<float>(_quadrilateral[1] - _quadrilateral[2], _quadrilateral[3] - _quadrilateral[2]);
		float d = Math::instance()->angle_degrees<float>(_quadrilateral[0] - _quadrilateral[3], _quadrilateral[2] - _quadrilateral[3]);

		float diff_a = std::abs(cnst::math::RIGHT_ANGLE - a);
		float diff_b = std::abs(cnst::math::RIGHT_ANGLE - b);
		float diff_c = std::abs(cnst::math::RIGHT_ANGLE - c);
		float diff_d = std::abs(cnst::math::RIGHT_ANGLE - d);

		if (((diff_a + diff_b + diff_c + diff_d) / _quadrilateral.size()) > cnst::processing::REJECTION_ANGLE)
			_quadrilateral.clear();
	}
	else	
		_quadrilateral.clear();
}

void Processing::_hough_transform(std::vector<cv::Vec4i> & _lines, cv::Mat const & _image)
{
	cv::Mat dst;

	cv::cvtColor(_image, dst, cv::COLOR_RGB2HSV_FULL);
	cv::inRange(_image, cnst::processing::LOWER_COLOR_INTERVAL_BOUND, cnst::processing::UPPER_COLOR_INTERVAL_BOUND, dst);

	cv::HoughLinesP(dst, _lines, cnst::processing::hough::RHO, cnst::processing::hough::THETA, cnst::processing::hough::THRESHOLD, cnst::processing::hough::MIN_LENGTH_LINE, cnst::processing::hough::MAX_GAP_LINE);
}

void Processing::_hough_lines_points(std::vector<cv::Point> & _points, std::vector<cv::Vec4i> const & _lines)
{
	for (auto it = _lines.begin(); it != _lines.end(); ++it)
	{
		cv::Vec4i const & p = * it;

		for (size_t k = 1; k < p.channels; k += 2)
			_points.push_back(cv::Point(p[k - 1], p[k]));
	}
}

void Processing::_generate_object_points(std::vector<cv::Point3d> &  _object_points, float _target_width, float _target_height)
{
	_object_points.emplace_back(_target_width / 2.0, -_target_height / 2.0, 0.0);
	_object_points.emplace_back(_target_width / 2.0, _target_height / 2.0, 0.0);
	_object_points.emplace_back(-_target_width / 2.0, _target_height / 2.0, 0.0);
	_object_points.emplace_back(-_target_width / 2.0, -_target_height / 2.0, 0.0);
}

void Processing::_estimate_rotation_vector_and_translation_vector(std::vector<double> & _rotation_vector, std::vector<double> & _translation_vector, std::vector<cv::Point3d> const & _object_points, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients)
{
	cv::Mat rvec = cv::Mat::zeros(3, 1, cv::DataType<double>::type);
	cv::Mat tvec = cv::Mat::zeros(3, 1, cv::DataType<double>::type);

	cv::solvePnP(_object_points, _quadrilateral, _camera_matrix, _distortion_coefficients, rvec, tvec);

	this->_matrix_3x1_to_vector3(_rotation_vector, rvec);
	this->_matrix_3x1_to_vector3(_translation_vector, tvec);
}

void Processing::_matrix_3x1_to_vector3(std::vector<double> & _vector, cv::Mat const & _matrix)
{
	for (size_t i = 0; i < _matrix.rows; i++)
		_vector[i] = _matrix.at<double>(0, i);
}

template <typename _T1, typename _T2>
void Processing::_vector_to_pointer(_T1 * _pointer, std::vector<_T2> const & _vector)
{
	for (size_t i = 0; i < _vector.size(); i++)
		_pointer[i] = (_T1)_vector[i];
}

template <typename _T1, typename _T2, int D>
void Processing::_point_vector_to_pointer(_T1 * _pointer, std::vector<_T2> const & _vector)
{
	for (size_t i = 0; i < _vector.size(); i++)
	{
		_pointer[i * D] = _vector[i].x;
		_pointer[i * D + 1] = _vector[i].y;
	}
}