template <typename _T>
class Singleton
{
public:
	static _T * instance()
	{
		if (!_instance)
			_instance = new _T();
		return _instance;
	}
	virtual ~Singleton()
	{
		_instance = 0;
	}

private:
	static _T * _instance;

protected:
	Singleton() { }
};

template <typename _T> _T * Singleton <_T>::_instance = 0;