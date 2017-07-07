#pragma once

#ifndef CV_PROCESSING_API_SINGLETON_H_
#define CV_PROCESSING_API_SINGLETON_H_

template <typename _T>
class Singleton
{
public:
	static _T * instance()
	{
		if (!m_instance)
			m_instance = new _T();
		return m_instance;
	}
	virtual ~Singleton()
	{
		m_instance = 0;
	}

private:
	static _T * m_instance;

protected:
	Singleton() { }
};

template <typename _T> _T * Singleton <_T>::m_instance = 0;

#endif // CV_PROCESSING_API_SINGLETON_H_