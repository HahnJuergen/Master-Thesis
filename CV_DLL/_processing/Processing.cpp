#include "pch.h"
#include "Processing.h"

void Processing::initialize_camera_related_matrices(cv::Mat & _cm, cv::Mat & _dc)
{
	_cm = cv::Mat();

	_cm.create(3, 3, CV_64F);

	/* hl values
	_cm.at<double>(0, 0) = 1006.224; // Fx
	_cm.at<double>(0, 1) = 0.0;
	_cm.at<double>(0, 2) = 448.0; // Cx
	_cm.at<double>(1, 0) = 0.0;
	_cm.at<double>(1, 1) = 1006.224; //Fy
	_cm.at<double>(1, 2) = 252.0; //Cy
	_cm.at<double>(2, 0) = 0.0;
	_cm.at<double>(2, 1) = 0.0;
	_cm.at<double>(2, 2) = 1.0;
	*/

	/* logitech hd 615 webcam values */
	_cm.at<double>(0, 0) = 675.306029; // Fx
	_cm.at<double>(0, 1) = 0.0;
	_cm.at<double>(0, 2) = 320.0; // Cx
	_cm.at<double>(1, 0) = 0.0;
	_cm.at<double>(1, 1) = 675.306029; //Fy
	_cm.at<double>(1, 2) = 240.0; //Cy
	_cm.at<double>(2, 0) = 0.0;
	_cm.at<double>(2, 1) = 0.0;
	_cm.at<double>(2, 2) = 1.0;

	_dc = cv::Mat();
	_dc.create(5, 1, CV_64F);

	/* hl values
	_dc.at<double>(0, 0) = -0.005678121;
	_dc.at<double>(0, 1) = -1.156654;
	_dc.at<double>(0, 2) = -0.001384972;
	_dc.at<double>(0, 3) = -0.003928866;
	_dc.at<double>(0, 4) = 9.449977;
	*/

	/* logitech hd 615 webcam values */
	_dc.at<double>(0, 0) = 0.005947;
	_dc.at<double>(0, 1) = -0.632893;
	_dc.at<double>(0, 2) = 0.001140;
	_dc.at<double>(0, 3) = -0.002175;
	_dc.at<double>(0, 4) = 1.806332;
}

void Processing::perform_probabilistic_hough_transform(std::vector<cv::Point> & _points, cv::Mat & _image)
{
	std::vector<cv::Vec4i> lines;

	this->_hough_transform(lines, _image);
	this->_hough_lines_points(_points, lines);
}

void Processing::approximate_polygon(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _points, uint32_t const _accuracy)
{
	std::vector<cv::Point> hull;

	this->_convex_hull(hull, _points);
	this->_approximate_quadrilateral(_quadrilateral, hull, _accuracy);
	this->_evaluate_detection_rejection(_quadrilateral);
}

void Processing::estimate_quadrilateral_pose(Quaternion<double> & _rotation, std::vector<double> & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients, float const _target_width, float const _target_height)
{
	if (_quadrilateral.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		cv::Mat object_points(4, 1, CV_32FC3);
		std::vector<double> rotation_vector(3);

		this->_generate_object_points(object_points, _target_width, _target_height);
		this->_estimate_rotation_vector_and_translation_vector(rotation_vector, _translation_vector, object_points, _quadrilateral, _camera_matrix, _distortion_coefficients);
		
		_rotation = Quaternion<double>::from_rodrigues_axis_angle(rotation_vector);
	}
}

uint32_t Processing::apply_data_to_out_datastructures(float * _out_quaternion, float * _out_tvec, Quaternion<double> const & _rotation, std::vector<double> const & _translation_vector, std::vector<cv::Point2d> const & _quadrilateral)
{
	if (_quadrilateral.size() == SUFFICIENT_NUMBER_CORNERS)
	{		
		_rotation >> _out_quaternion;

		this->_vector_to_pointer(_out_tvec, _translation_vector);

		return 0;
	}
	else
	{
		return 1;
	}
}

void Processing::_convex_hull(std::vector<cv::Point> & _hull, std::vector<cv::Point> const & _points)
{
	if (_points.size() > (SUFFICIENT_NUMBER_CORNERS - 1))
		cv::convexHull(_points, _hull);
}

void Processing::_approximate_quadrilateral(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> & _hull, uint32_t const _accuracy)
{
	this->_approximate_polygon_from_convex_hull(_quadrilateral, _hull);
	this->_unify_quadrilateral_corner_descriptors(_quadrilateral);

	for (uint32_t i = 0; i < _accuracy; i++)
	{
		std::vector<cv::Vec4d> fit_edges;
		std::vector<cv::Point> intersections;

		this->_compute_edges(fit_edges, _hull, _quadrilateral);
		if (!this->_intersect_fitted_edges(intersections, fit_edges)) break;
		this->_accumulate_convex_hull(_hull, intersections);
		this->_approximate_polygon_from_convex_hull(_quadrilateral, _hull);
		this->_unify_quadrilateral_corner_descriptors(_quadrilateral);
	}	
}

void Processing::_compute_edges(std::vector<cv::Vec4d> & _edges, std::vector<cv::Point> const & _hull, std::vector<cv::Point2d> const & _quadrilateral)
{
	if (_quadrilateral.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		std::vector<std::vector<cv::Point>> edges;

		this->_retrieve_points_of_edges(edges, _hull, _quadrilateral);
		this->_fit_edge_line(_edges, edges);
	}	
}

void Processing::_retrieve_points_of_edges(std::vector<std::vector<cv::Point>> & _edges, std::vector<cv::Point> const & _hull, std::vector<cv::Point2d> const & _quadrilateral)
{
	_edges.push_back(std::vector<cv::Point>());

	uint32_t i = 0;

	for (auto p = _hull.begin(); p != _hull.end(); ++p)
	{
		for (auto q = _quadrilateral.begin(); q != _quadrilateral.end(); ++q)
		{
			_edges[i].push_back(* p);

			if (p->x == q->x && p->y == q->y)
			{
				_edges.push_back(std::vector<cv::Point>());
				_edges[++i].push_back(* p);
			}
		}
	}

	if (_edges.size() == SUFFICIENT_NUMBER_CORNERS + 1)
		_edges.erase(_edges.begin());
}

void Processing::_fit_edge_line(std::vector<cv::Vec4d> & _fit_edges, std::vector<std::vector<cv::Point>> const & _edges)
{
	if (_edges.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		for (auto e = _edges.begin(); e != _edges.end(); ++e)
		{
			cv::Vec4d line;

			cv::fitLine(* e, line, cv::DIST_WELSCH, CHOOSE_OPTIMAL, EPSILON_RADIUS, EPSILON_ANGLE);

			_fit_edges.push_back(line);
		}
	}
}

void Processing::_accumulate_convex_hull(std::vector<cv::Point> & _hull, std::vector<cv::Point> const & _intersections)
{
	if (_intersections.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		for (auto it = _intersections.begin(); it != _intersections.end(); ++it)
			_hull.push_back(*it);		

		cv::convexHull(_hull, _hull);
	}	
}

void Processing::_approximate_polygon_from_convex_hull(std::vector<cv::Point2d> & _quadrilateral, std::vector<cv::Point> const & _hull)
{
	if (_hull.size() > SUFFICIENT_NUMBER_CORNERS - 1)
	{
		double epsilon = ARC_LENGTH_MULTIPLICATOR * cv::arcLength(_hull, true);
		cv::approxPolyDP(_hull, _quadrilateral, epsilon, true);
	}	
}

void Processing::_unify_quadrilateral_corner_descriptors(std::vector<cv::Point2d> & _quadrilateral)
{
	if (_quadrilateral.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		cv::Point2d tlc, blc, brc, trc;

		auto comparator_x = [&](cv::Point2d const & p, cv::Point const & q) -> bool { return p.x < q.x; };

		std::sort(_quadrilateral.begin(), _quadrilateral.end(), comparator_x);

		this->_classify_extreme_point_pair_vertical_edge<cv::Point2d, float>(tlc, blc, _quadrilateral[0], _quadrilateral[1], _quadrilateral[0].y, _quadrilateral[1].y);
		this->_classify_extreme_point_pair_vertical_edge<cv::Point2d, float>(brc, trc, _quadrilateral[2], _quadrilateral[3], Math::instance()->distance<float, cv::Point2d, 2>(tlc, _quadrilateral[2]), Math::instance()->distance<float, cv::Point2d, 2>(tlc, _quadrilateral[3]));

		_quadrilateral[0] = tlc;
		_quadrilateral[1] = blc;
		_quadrilateral[2] = brc;
		_quadrilateral[3] = trc;
	}	
}

void Processing::_evaluate_detection_rejection(std::vector<cv::Point2d> & _quadrilateral)
{
	if (_quadrilateral.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		float a = Math::instance()->angle_degrees<float, cv::Point2d>(_quadrilateral[3] - _quadrilateral[0], _quadrilateral[1] - _quadrilateral[0]);
		float b = Math::instance()->angle_degrees<float, cv::Point2d>(_quadrilateral[0] - _quadrilateral[1], _quadrilateral[2] - _quadrilateral[1]);
		float c = Math::instance()->angle_degrees<float, cv::Point2d>(_quadrilateral[1] - _quadrilateral[2], _quadrilateral[3] - _quadrilateral[2]);
		float d = Math::instance()->angle_degrees<float, cv::Point2d>(_quadrilateral[0] - _quadrilateral[3], _quadrilateral[2] - _quadrilateral[3]);

		float diff_a = std::abs(RIGHT_ANGLE - a);
		float diff_b = std::abs(RIGHT_ANGLE - b);
		float diff_c = std::abs(RIGHT_ANGLE - c);
		float diff_d = std::abs(RIGHT_ANGLE - d);

		if (((diff_a + diff_b + diff_c + diff_d) / _quadrilateral.size()) > REJECTION_ANGLE)
			_quadrilateral.clear();
	}
	else	
		_quadrilateral.clear();
}

void Processing::_hough_transform(std::vector<cv::Vec4i> & _lines, cv::Mat const & _image)
{
	cv::Mat dst;

	cv::GaussianBlur(_image, _image, BLURRING_KERNEL_SIZE, SIGMA_X, SIGMA_Y);
	cv::inRange(_image, LOWER_COLOR_INTERVAL_BOUND, UPPER_COLOR_INTERVAL_BOUND, dst);
	cv::HoughLinesP(dst, _lines, RHO, THETA, THRESHOLD, MIN_LENGTH_LINE, MAX_GAP_LINE);
}

void Processing::_hough_lines_points(std::vector<cv::Point> & _points, std::vector<cv::Vec4i> const & _lines)
{
	for (auto it = _lines.begin(); it != _lines.end(); ++it)
		for (uint32_t k = 1; k < it->channels; k += 2)
			_points.push_back(cv::Point((*it)[k - 1], (*it)[k]));	
}

void Processing::_generate_object_points(cv::Mat & _object_points, float const _target_width, float const _target_height)
{
	_object_points.ptr<cv::Vec3f>(0)[0] = cv::Vec3f(-_target_width / 2 + 0.16f, -_target_height / 2 + 0.1f, 0);
	_object_points.ptr<cv::Vec3f>(0)[1] = cv::Vec3f(-_target_width / 2 + 0.16f, _target_height / 2 + 0.1f, 0);
	_object_points.ptr<cv::Vec3f>(0)[2] = cv::Vec3f(_target_width / 2 + 0.16f, _target_height / 2 + 0.1f, 0);
	_object_points.ptr<cv::Vec3f>(0)[3] = cv::Vec3f(_target_width / 2 + 0.16f, -_target_height / 2 + 0.1f, 0);
}

void Processing::_estimate_rotation_vector_and_translation_vector(std::vector<double> & _rotation_vector, std::vector<double> & _translation_vector, cv::Mat const & _object_points, std::vector<cv::Point2d> const & _quadrilateral, cv::Mat const & _camera_matrix, cv::Mat const & _distortion_coefficients)
{
	cv::Mat rvec = cv::Mat::zeros(3, 1, CV_64FC3);
	cv::Mat tvec = cv::Mat::zeros(3, 1, CV_64FC3);

	cv::solvePnP(_object_points, _quadrilateral, _camera_matrix, _distortion_coefficients, rvec, tvec, false, cv::SOLVEPNP_ITERATIVE);

	this->_matrix_to_vector(_rotation_vector, rvec);
	this->_matrix_to_vector(_translation_vector, tvec);
}

void Processing::_matrix_to_vector(std::vector<double> & _vector, cv::Mat const & _matrix)
{
	for (uint32_t i = 0; i < _matrix.cols; i++)
		for (uint32_t k = 0; k < _matrix.rows; k++)
			_vector[i * _matrix.rows + k] = _matrix.at<double>(i, k);
}

bool Processing::_intersect_fitted_edges(std::vector<cv::Point> & _intersections, std::vector<cv::Vec4d> const & _fit_edges)
{
	if (_fit_edges.size() == SUFFICIENT_NUMBER_CORNERS)
	{
		cv::Point p, q, r, s;

		cv::Point p0(_fit_edges[0][2] - _fit_edges[0][0], _fit_edges[0][3] - _fit_edges[0][1]);
		cv::Point q0(_fit_edges[0][2] + _fit_edges[0][0], _fit_edges[0][3] + _fit_edges[0][1]);

		cv::Point p1(_fit_edges[1][2] - _fit_edges[1][0], _fit_edges[1][3] - _fit_edges[1][1]);
		cv::Point q1(_fit_edges[1][2] + _fit_edges[1][0], _fit_edges[1][3] + _fit_edges[1][1]);

		cv::Point p2(_fit_edges[2][2] - _fit_edges[2][0], _fit_edges[2][3] - _fit_edges[2][1]);
		cv::Point q2(_fit_edges[2][2] + _fit_edges[2][0], _fit_edges[2][3] + _fit_edges[2][1]);

		cv::Point p3(_fit_edges[3][2] - _fit_edges[3][0], _fit_edges[3][3] - _fit_edges[3][1]);
		cv::Point q3(_fit_edges[3][2] + _fit_edges[3][0], _fit_edges[3][3] + _fit_edges[3][1]);

		if (!Math::instance()->intersection(p, p0, q0, p1, q1)
			|| !Math::instance()->intersection(q, p0, q0, p3, q3)
			|| !Math::instance()->intersection(r, p2, q2, p1, q1)
			|| !Math::instance()->intersection(s, p2, q2, p3, q3))
			return false;

		_intersections = std::vector<cv::Point>{ p, q, r, s };

		return true;
	}

	return false;
}

template<typename _T1, typename _T2>
void Processing::_classify_extreme_point_pair_vertical_edge(_T1 & _t1, _T1 & _t2, _T1 const & _s1, _T1 const & _s2, _T2 const & _c1, _T2 const & _c2)
{
	if (_c1 > _c2)
	{
		_t1 = _s1;
		_t2 = _s2;
	}
	else
	{
		_t1 = _s2;
		_t2 = _s1;;
	}
}

template <typename _T1, typename _T2>
void Processing::_vector_to_pointer(_T1 * _pointer, std::vector<_T2> const & _vector)
{
	for (uint32_t i = 0; i < _vector.size(); i++)
		_pointer[i] = (_T1)_vector[i];
}

template <typename _T1, typename _T2, uint32_t D>
void Processing::_point_vector_to_pointer(_T1 * _pointer, std::vector<_T2> const & _vector)
{
	for (uint32_t i = 0; i < _vector.size(); i++)
	{
		_pointer[i * D] = _vector[i].x;
		_pointer[i * D + 1] = _vector[i].y;
	}
}