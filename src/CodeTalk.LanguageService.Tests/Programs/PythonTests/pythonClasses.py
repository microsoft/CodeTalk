# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
class adder:
    def __init__(self, a, b):
        self.a=a
        self.b=b

    def getFirstValue(self):
        return self.a

    def getSecondValue(self):
        return self.b

    def add(self):
        return self.a+self.b

#method that is not in any class:
def outer_method():
    return "bla"

class SecondClass:
    def __init__(self):
        pass

    def subtract(a,b):
        return a+b