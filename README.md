#Fishtrace
##Produced using winui3

You can click the record button and drag your image. The trace can be recorded and replayed. But this is not the core part, I had implemented two methods to smooth the trace. One of them is done using polynomial fit and the other is done by Fast Fourier Transform(FFT). Polynomial fit function will fit a polynomial curve to the trace and create a new trace from the curve. Or you can try FFT, which will convert your trace to frequency domain and remove some high-frequencies. 
