# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
class adder:
    def __init__(self, a, b):
        self.a=a
        self.b=b
        self.list = [1,2,3,4,5,6]

    def getFirstValue(self):
        return self.a

    def getSecondValue(self):
        return self.b

    def add(self):
        a=0
        for i in self.list:
            self.a = self.a+ i
            return self.a

    def addEvens(self):
        l = len(self.list)
        i=0
        sum=0
        while i in range(l):
            if self.list[i]%2 == 0:
                sum = sum + self.list[i]
        return sum

    def returnOdds(self):
        return [odds for num in self.list if num%2 != 0]

    
#method that is not in any class:
def outer_method():
    return "bla"

class SecondClass:
    def __init__(self):
        pass

    def subtract(a,b):
        return a+b