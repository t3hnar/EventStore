#pragma once
#include "CompiledScript.h"

namespace js1 {

	class PreludeScope
	{
	public:
		// ignore null prelude script - likely from load module callback and isolate is already set
		PreludeScope(CompiledScript *prelude) :
			isolate(prelude == NULL ? v8::Isolate::GetCurrent() : prelude->get_isolate())
		{
			isolate->Enter();
			CompiledScript::isolate_add_ref(isolate);
		}
		~PreludeScope()
		{
			bool do_delete = false;
			size_t counter = CompiledScript::isolate_release(isolate);
			do_delete = counter == 0;
			isolate->Exit();
			if (do_delete)
				isolate->Dispose();
		}
	private:
		v8::Isolate *isolate;
		PreludeScope(const PreludeScope &);
		PreludeScope& operator=(const PreludeScope &);
	};
}
